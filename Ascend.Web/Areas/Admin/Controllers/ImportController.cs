
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using LumenWorks.Framework.IO.Csv;
using Ascend.Core.Services.Import;
using Net.SourceForge.Koogra;
using RedBranch.Hammock;

using Ascend.Core;
using Ascend.Infrastructure;
using Ascend.Core.Repositories;
using Ascend.Core.Services;

namespace Ascend.Web.Areas.Admin.Controllers
{
	#region ImportCreateModel

	public class ImportCreateModel
	{
		[Required, StringLength(100)] public string Location { get; set; }
        [UIHint("Enum")] public ImportType Type { get; set; }

		public Import CreateImport()
		{
			return new Import {
                Type = Type,
				Location = Location,
			};
		}
	}
	
	#endregion
	#region ImportEditModel
	
	public class ImportEditModel
	{
        [UIHint("Enum")] public ImportType Type { get; set; }
		public string Location { get; set; }
        [DisplayName("Send email notifications.")] public bool Notify { get; set; }
        public ImportColumn[] Columns { get; set; }
        public List<ImportAttempt> Attempts { get; set; }

		public static ImportEditModel FromDomain(Import i)
		{
			return new ImportEditModel {
                Type = i.Type,
				Location = i.Location,
                Notify = i.Notify,
                Columns = i.Columns ?? new ImportColumn[0],
                Attempts = i.Attempts ?? new List<ImportAttempt>(),
			};
		}
	
		public void Apply(Import i)
		{
            i.Type = Type;
            i.Notify = Notify;
            for (var j=0; j<i.Columns.Length; j++)
            {
                i.Columns[j].Target = Columns[j].Target;
                i.Columns[j].CustomTarget = Columns[j].CustomTarget;
            }
		}
	}
	
	#endregion
    #region RandomizerModel

    public class RandomizerModel
    {
        public int Users { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string EmailDomain { get; set; }
    }

    #endregion
	
    public partial class ImportController : AdminController
    {
        public IImportRepository Imports { get; set; }
        public IEventTicketingService TicketingService { get; set; }
        public IRepository<Error> Errors { get; set; }

        public IImportService<User> UserImportService { get; set; }
        public IImportService<Award> PointsImportService { get; set; }
        public IImportService<Product> ProductImportService { get; set; }

        protected IImportSource GetSourceForImport(Import i)
        {
            if (i.Location.EndsWith(".csv"))
            {
                return new CsvImportSource(i.Location.ToAbsoluteUrl().ToString());
            }
            if (i.Location.EndsWith(".xls") ||
                i.Location.EndsWith((".xlsx")))
            {
                return new ExcelImportSource(i.Location.ToAbsoluteUrl().ToString());
            }

            throw new NotSupportedException();
        }

		[HttpGet]
		public virtual ActionResult Index()
        {
            return View(Imports.All().WithDocuments());
        }

        [HttpPost]
        public virtual ActionResult Index(ImportCreateModel i)
        {
			ViewData["i"] = i;
			if (!ModelState.IsValid)
			{
                return View(Imports.All().WithDocuments());
			}
			
			var x = i.CreateImport();
            try
            {
                // attempt to load file
                var source = GetSourceForImport(x);
                var headers = source.Fields;
                x.Columns = headers.Select(h => new ImportColumn {
                    Name = h,
                }).ToArray();

                // look back at previous imports for matching columns
                foreach (var recent in Imports.FindRecentImports())
                {
                    if (null != recent.Columns &&
                        recent.Columns.Length == x.Columns.Length &&
                        Enumerable.Range(0, recent.Columns.Length-1)
                                  .All(j => recent.Columns[j].Name == x.Columns[j].Name))
                    {
                        x.Type = recent.Type;
                        for (int j=0; j<recent.Columns.Length; j++)
                        {
                            x.Columns[j].Target = recent.Columns[j].Target;
                            x.Columns[j].CustomTarget = recent.Columns[j].CustomTarget;
                        }
                        break;
                    }
                }

                // try to find matching column names
                var targets = new string[0];
                switch (x.Type)
                {
                    case ImportType.Users: targets = Enum.GetNames(typeof(UserColumnMappingTargets)); break;
                    case ImportType.Points: targets = Enum.GetNames(typeof(PointsColumnMappingTargets)); break;
                    case ImportType.Products: targets = Enum.GetNames(typeof(ProductColumnMappingTargets)); break;
                }
                foreach (var c in x.Columns)
                {
                    if (c.Target == null)
                    {
                        var lower = c.Name.ToLowerInvariant();
                        var a = targets.FirstOrDefault(b => b.ToLowerInvariant().Contains(lower)) ??
                                targets.FirstOrDefault(b => lower.Contains(b.ToLowerInvariant()));
                        c.Target = a;
                    }

                }
                Imports.Save(x);
                return RedirectToAction(Actions.Edit, new { id = x.Document.Id });
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(Imports.All().WithDocuments());
            }
        }

		[HttpGet]
        public virtual ActionResult Edit(string id)
		{
		    SupplyTargets();
		    return View(ImportEditModel.FromDomain(Imports.Get(id)));
		}

        private void SupplyTargets()
        {
            ViewData["targets"] = 
                new [] {
                           Enum.GetNames(typeof(UserColumnMappingTargets)),
                           Enum.GetNames(typeof(PointsColumnMappingTargets)),
                           Enum.GetNames(typeof(ProductColumnMappingTargets))
                       };
        }

        [HttpPost]
        public virtual ActionResult Edit(string id, ImportEditModel i, bool import)
        {
			var x = Imports.Get(id);
			if (!ModelState.IsValid)
			{
                SupplyTargets();
				return View(i);
			}
            try
            {
                i.Apply(x);
				Imports.Save(x);

                if (import)
                {
                    // import time
                    ImportResult r = null;
                    var source = GetSourceForImport(x);
                    switch (x.Type)
                    {
                        case ImportType.Users:
                            r = UserImportService.Import(x, source, true);
                            break;
                        case ImportType.Points:
                            r = PointsImportService.Import(x, source, true);
                            break;
                        case ImportType.Products:
                            r = ProductImportService.Import(x, source, true);
                            break;
                        default:
                            throw new Exception("Unknown import type, " + x.Type + ".");
                    }

                    // record the attempt.. its a truncated version of the result suitable
                    // for long term storage.
                    if (null == x.Attempts)
                    {
                        x.Attempts = new List<ImportAttempt>();
                    }
                    x.Attempts.Add(r.ToAttempt());
                    Imports.Save(x);

                    // but the show the user a very detailed output
                    if ((null == r.Problems ||
                         r.Problems.Count == 0) &&
                        x.Type == ImportType.Products)
                    {
                        Notifier.Notify(
                            Severity.Success, 
                            "Import complete.",
                            String.Format("{0} rows succesfully imported.", r.Rows.Count),
                            null);

                        return RedirectToAction(Actions.Index);
                    }

                    Session["ImportResult"] = r;
                    return RedirectToAction(Actions.Review);
                }

                return RedirectToAction(Actions.Index);
            }
            catch (Exception ex)
            {
                SupplyTargets();
				Notifier.Notify(ex);
                return View(i);
            }
        }

        [HttpGet]
        public virtual ActionResult Review()
        {
            return View((ImportResult)Session["ImportResult"]);
        }
        
        #region TicketJones

        [HttpGet]
        public virtual ActionResult Events()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult Events(FormCollection form)
        {
            try
            {
                TicketingService.Repopulate();
                Notifier.Notify(Severity.Success, "Event data repopulated.", null, null);
                return RedirectToAction(MVC.Admin.Home.Index());
            }
            catch (Exception ex)
            {
                Notifier.Notify(ex);
            }
            return View();
        }

        #endregion
        #region Reset

        public ITransactionRepository Transactions { get; set; }
        public IAccountRepository Accounts { get; set; }
        
        [HttpGet]
        public virtual ActionResult Reset()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult Reset(bool confirm)
        {
            if (!confirm)
                return View();

            // delete users
            Users.All().WithDocuments().Each(x => {
                if (new [] { "user-nickn", "user-anthonys", "user-beaub", "user-larryh" }.Contains(x.Document.Id))
                {
                    x.PointsAccount = null;
                    x.BudgetAccount = null;
                    x.Group = null;
                    x.ManagedGroups = null;
                    x.Manager = null; 
                    Users.Save(x);    
                }
                else
                {
                    Users.Delete(x);
                }
            });
            UserAwards.All().WithDocuments().Each(x => UserAwards.Delete(x));
            QuizResults.All().WithDocuments().Each(x => QuizResults.Delete(x));
            Transactions.All().WithDocuments().Each(x => Transactions.Delete(x));
            Errors.All().WithDocuments().Each(x => Errors.Delete(x));
            Accounts.All().WithDocuments().Each(x => {
                if (x.Document.Id == Application.GeneralControlAccount ||
                    x.Document.Id == Application.GeneralExpenseAccount)
                {
                    return;
                }
                Accounts.Delete(x);
            });
            Orders.All().WithDocuments().Each(x => Orders.Delete(x));

            Notifier.Notify(Severity.Success, "All user evidence removed. This world is ready for repopulation.", null, null);
            return RedirectToAction(MVC.Admin.Home.Index());
        }

        #endregion
        #region Randomizer

        #region Firstnames

        readonly static string[] Firstnames = new[] {
            "Jacob", "Emily",
            "Michael", "Madison",
            "Joshua", "Emma",
            "Matthew", "Hannah",
            "Christopher", "Olivia",
            "Andrew", "Abigail",
            "Daniel", "Isabella",
            "Ethan", "Ashley",
            "Joseph", "Samantha",
            "William", "Elizabeth",
            "Anthony", "Alexis",
            "Nicholas", "Sarah",
            "David", "Alyssa",
            "Alexander", "Grace",
            "Ryan", "Sophia",
            "Tyler", "Taylor",
            "James", "Brianna",
            "John", "Lauren",
            "Jonathan", "Ava",
            "Brandon", "Kayla",
            "Christian", "Jessica",
            "Dylan", "Natalie",
            "Zachary", "Chloe",
            "Noah", "Anna",
            "Samuel", "Victoria",
            "Benjamin", "Hailey",
            "Nathan", "Mia",
            "Logan", "Sydney",
            "Justin", "Jasmine",
            "Jose", "Morgan",
            "Gabriel", "Julia",
            "Austin", "Destiny",
            "Kevin", "Rachel",
            "Caleb", "Megan",
            "Robert", "Kaitlyn",
            "Elijah", "Katherine",
            "Thomas", "Jennifer",
            "Jordan", "Savannah",
            "Cameron", "Ella",
            "Hunter", "Alexandra",
            "Jack", "Haley",
            "Angel", "Allison",
            "Isaiah", "Maria",
            "Jackson", "Nicole",
            "Evan", "Mackenzie",
            "Luke", "Brooke",
            "Jason", "Makayla",
            "Isaac", "Kaylee",
            "Mason", "Lily",
            "Aaron", "Stephanie",
            "Connor", "Andrea",
            "Gavin", "Faith",
            "Kyle", "Amanda",
            "Jayden", "Katelyn",
            "Aidan", "Kimberly",
            "Juan", "Madeline",
            "Luis", "Gabrielle",
            "Charles", "Zoe",
            "Aiden", "Trinity",
            "Adam", "Alexa",
            "Brian", "Mary",
            "Eric", "Jenna",
            "Lucas", "Lillian",
            "Sean", "Paige",
            "Nathaniel", "Kylie",
            "Alex", "Gabriella",
            "Adrian", "Rebecca",
            "Carlos", "Jordan",
            "Bryan", "Sara",
            "Ian", "Addison",
            "Jesus", "Michelle",
            "Owen", "Riley",
            "Julian", "Vanessa",
            "Cole", "Angelina",
            "Landon", "Leah",
            "Diego", "Caroline",
            "Steven", "Sofia",
            "Chase", "Audrey",
            "Timothy", "Maya",
            "Jeremiah", "Avery",
            "Sebastian", "Evelyn",
            "Xavier", "Autumn",
            "Devin", "Amber",
            "Cody", "Ariana",
            "Seth", "Jocelyn",
            "Hayden", "Claire",
            "Blake", "Jada",
            "Richard", "Danielle",
            "Carter", "Bailey",
            "Wyatt", "Isabel",
            "Dominic", "Arianna",
            "Antonio", "Sierra",
            "Jaden", "Mariah",
            "Miguel", "Aaliyah",
            "Brayden", "Melanie",
            "Patrick", "Erin",
            "Alejandro", "Nevaeh",
            "Carson", "Brooklyn",
            "Jesse", "Marissa",
            "Tristan", "Jacqueline",
            "Trevor", "Melissa",
            "Victor", "Molly",
            "Henry", "Isabelle",
            "Jake", "Shelby",
            "Liam", "Leslie",
            "Jared", "Angela",
            "Bryce", "Amelia",
            "Riley", "Madelyn",
            "Colin", "Katie",
            "Mark", "Catherine",
            "Jeremy", "Jade",
            "Garrett", "Diana",
            "Caden", "Briana",
            "Marcus", "Breanna",
            "Vincent", "Gabriela",
            "Kenneth", "Amy",
            "Parker", "Courtney",
            "Kaleb", "Christina",
            "Brady", "Mya",
            "Kaden", "Adriana",
            "Jorge", "Kathryn",
            "Oscar", "Kennedy",
            "Joel", "Gracie",
            "Colton", "Sophie",
            "Tanner", "Alexandria",
            "Ashton", "Daniela",
            "Paul", "Aubrey",
            "Josiah", "Gianna",
            "Eduardo", "Laura",
            "Edward", "Angel",
            "Ivan", "Lydia",
            "Cooper", "Natalia",
            "Maxwell", "Peyton",
            "Stephen", "Margaret",
            "Preston", "Valeria",
            "Alan", "Cheyenne",
            "Dakota", "Miranda",
            "Alexis", "Mikayla",
            "Nicolas", "Kelsey",
            "Spencer", "Alicia",
            "Grant", "Ana",
            "George", "Caitlin",
            "Omar", "Layla",
            "Ricardo", "Sabrina",
            "Cristian", "Jillian",
            "Collin", "Daisy",
            "Derek", "Lindsey",
            "Dalton", "Naomi",
            "Gage", "Mckenzie",
            "Francisco", "Kelly",
            "Jeffrey", "Ashlyn",
            "Levi", "Summer",
            "Brendan", "Skylar",
            "Peter", "Charlotte",
            "Shane", "Cassidy",
            "Travis", "Alexia",
            "Max", "Payton",
            "Micah", "Valerie",
            "Fernando", "Karen",
            "Nolan", "Rylee",
            "Eli", "Cassandra",
            "Damian", "Tiffany",
            "Javier", "Kylee",
            "Andres", "Ruby",
            "Conner", "Kate",
            "Shawn", "Kendall",
            "Peyton", "Caitlyn",
            "Braden", "Jordyn",
            "Cesar", "Sadie",
            "Bradley", "Jayla",
            "Manuel", "Alondra",
            "Brody", "Juliana",
            "Ayden", "Erica",
            "Giovanni", "Eva",
            "Jonah", "Kiara",
            "Erik", "Genesis",
            "Emmanuel", "Crystal",
            "Edgar", "Angelica",
            "Mario", "Bianca",
            "Devon", "Reagan",
            "Edwin", "Zoey",
            "Johnathan", "Giselle",
            "Jalen", "Hope",
            "Erick", "Brittany",
            "Trenton", "Chelsea",
            "Hector", "Lucy",
            "Wesley", "Serenity",
            "Raymond", "Veronica",
            "Gregory", "Alana",
            "Malachi", "Karina",
            "Colby", "Abby",
            "Sergio", "Erika",
            "Abraham", "Jasmin",
            "Marco", "Delaney",
            "Donovan", "Jazmin",
            "Clayton", "Makenzie",
            "Martin", "Maggie",
            "Leonardo", "Jamie",
            "Elias", "Adrianna",
            "Roberto", "Carly",
            "Oliver", "Julianna",
            "Dillon", "Karla",
            "Taylor", "Liliana",
            "Damien", "Mckenna",
            "Jaylen", "Kyla",
            "Bryson", "Ellie",
            "Josue", "Ariel",
            "Corey", "Kyra",
            "Andre", "Lilly",
            "Andy", "Esmeralda",
            "Mitchell", "Bethany",
            "Harrison", "Alejandra",
            "Dawson", "Mariana",
            "Dustin", "Nadia",
            "Ty", "Cynthia",
            "Drew", "Elena",
            "Miles", "Camila",
            "Scott", "Monica",
            "Calvin", "Amaya",
            "Pedro", "Brenda",
            "Israel", "Meghan",
            "Avery", "Heather",
            "Ruben", "Aliyah",
            "Jace", "Hayley",
            "Brett", "Camryn",
            "Dominick", "Mallory",
            "Jakob", "Elise",
            "Rafael", "Kendra",
            "Malik", "Hanna",
            "Frank", "Vivian",
            "Zane", "Makenna",
            "Troy", "Rebekah",
            "Alec", "Jazmine",
            "Griffin", "Haylee",
            "Johnny", "Fatima",
            "Trey", "Josephine",
            "Roman", "Jayden",
            "Chance", "Keira",
            "Ronald", "Bella",
            "Cade", "Michaela",
            "Skyler", "Lizbeth",
            "Gerardo", "Kara",
            "Phillip", "Desiree",
            "Keegan", "Macy",
            "Raul", "Laila",
            "Derrick", "Savanna",
            "Marcos", "Kristen",
            "Jaxon", "Diamond",
            "Camden", "Shannon",
            "Simon", "Alison",
            "Payton", "Joanna",
            "Trent", "Nina",
            "Armando", "Lindsay",
            "Drake", "Allyson",
            "Grayson", "Tessa",
            "Enrique", "Reese",
            "Julio", "Julie",
            "Braxton", "Guadalupe",
            "Cayden", "Selena",
            "Keith", "Dakota",
            "Jaiden", "Katelynn",
            "Allen", "Ciara",
            "Santiago", "Kaitlin",
            "Darius", "Carmen",
            "Casey", "Annabelle",
            "Brock", "Alaina",
            "Dante", "Clara",
            "Donald", "Cameron",
            "Jaime", "Kira",
            "Kameron", "Kailey",
            "Kai", "Cecilia",
            "Fabian", "Holly",
            "Tucker", "Heaven",
            "Chandler", "Camille",
            "Angelo", "Claudia",
            "Landen", "Asia",
            "Leo", "April",
            "Corbin", "Kayleigh",
            "Kyler", "Nancy",
            "Brennan", "Tatiana",
            "Kayden", "Carolina",
            "Emanuel", "Miriam",
            "Gustavo", "Allie",
            "Zackary", "Esther",
            "Lukas", "Aniyah",
            "Zion", "Piper",
            "Brenden", "Celeste",
            "Alberto", "Anastasia",
            "Lance", "Serena",
            "Tyson", "Jaden",
            "Mathew", "Eleanor",
            "Jerry", "Britney",
            "Myles", "Madeleine",
            "Quinn", "Julissa",
            "Saul", "Josie",
            "Jimmy", "Callie",
            "Pablo", "Paris",
            "Dennis", "Raven",
            "Hudson", "Kamryn",
            "Danny", "Alivia",
            "Ezekiel", "Kathleen",
            "Philip", "Brooklynn",
            "Louis", "Skyler",
            "Randy", "Sandra",
            "Theodore", "Melody",
            "Axel", "Cindy",
            "Kaiden", "Stella",
            "Emilio", "Ashlynn",
            "Lane", "Nora",
            "Lorenzo", "Katrina",
            "Arturo", "Bridget",
            "Tony", "Paola",
            "Jonathon", "Sienna",
            "Curtis", "Aniya",
            "Mateo", "Cadence",
            "Asher", "Wendy",
            "Xander", "Eliana",
            "Kobe", "Heidi",
            "Nickolas", "Kristina",
            "Cory", "Alayna",
            "Albert", "Kirsten",
            "Larry", "Natasha",
            "Marc", "Priscilla",
            "Rylan", "Daniella",
            "Alfredo", "Eden",
            "Gary", "Shayla",
            "Sawyer", "Patricia",
            "Aden", "Alissa",
            "Damon", "Jayda",
            "Darren", "Kaylie",
            "Tristen", "Rachael",
            "Douglas", "Rose",
            "Ismael", "Casey",
            "Chad", "Georgia",
            "Ryder", "Leila",
            "Esteban", "Jadyn",
            "Jayson", "Nayeli",
            "Bryant", "Yesenia",
            "Maximilian", "Eliza",
            "Dallas", "Denise",
            "Adan", "Brenna",
            "Brayan", "Alina",
            "Amir", "Meredith",
            "Moises", "Ivy",
            "Braeden", "Christine",
            "Quentin", "Ashlee",
            "Joe", "Emely",
            "Arthur", "Ruth",
            "Ricky", "Kassandra",
            "Marvin", "Tori",
            "Ramon", "Tara",
            "Jay", "Emilee",
            "Abel", "Cierra",
            "Walter", "Imani",
            "Maddox", "Helen",
            "Morgan", "Annika",
            "Jude", "Kiana",
            "Rodrigo", "Hayden",
            "Maximus", "Leilani",
            "Ernesto", "Lauryn",
            "Salvador", "Anahi",
            "Nikolas", "Mercedes",
            "Kristopher", "Izabella",
            "Julius", "Aurora",
            "Russell", "Annie",
            "Zander", "Nia",
            "Charlie", "Sidney",
            "Joaquin", "Yasmin",
            "Lawrence", "Madalyn",
            "Dean", "Fiona",
            "Keaton", "Lexi",
            "Ali", "Logan",
            "Ezra", "Talia",
            "Camron", "Iris",
            "Mauricio", "Kiera",
            "Orlando", "Violet",
            "Chris", "Tatum",
            "Lincoln", "Harley",
            "Micheal", "Brittney",
            "Weston", "Bryanna",
            "Bailey", "Marisa",
            "Carl", "Sasha",
            "Felix", "Rylie",
            "Marshall", "Dana",
            "Hugo", "Angie",
            "Reece", "Rosa",
            "Maurice", "Dulce",
            "Isiah", "Anne",
            "Davis", "Kiley",
            "Emiliano", "Marisol",
            "Shaun", "Joselyn",
            "Eddie", "Kassidy",
            "Holden", "Lila",
            "Jonas", "Phoebe",
            "Elliot", "Clarissa",
            "Graham", "Genevieve",
            "Deandre", "Marina",
            "Tommy", "Lucia",
            "Justice", "Alice",
            "Declan", "Kyleigh",
            "Zachery", "Perla",
            "Terry", "Ryleigh",
            "Issac", "Alexus",
            "Bennett", "Lesly",
            "Kade", "Hailee",
            "Caiden", "Itzel",
            "Javon", "Marley",
            "Luca", "Hallie",
            "Jeffery", "Madisyn",
            "Dane", "Brynn",
            "Zachariah", "Scarlett",
            "Rodney", "Lacey",
            "Silas", "Cora",
            "Brent", "Gloria",
            "Reese", "Linda",
            "Gael", "Lisa",
            "Tate", "Whitney",
            "Walker", "Baylee",
            "Easton", "Halle",
            "Roger", "Shania",
            "Jon", "Elisabeth",
            "Branden", "Cristina",
            "Uriel", "Kaylin",
            "Kelvin", "Malia",
            "Melvin", "Sage",
            "Jaxson", "Madyson",
            "Braydon", "Deanna",
            "Reid", "Raquel",
            "Skylar", "Tabitha",
            "Yahir", "Krystal",
            "Guillermo", "London",
            "Jadon", "Francesca",
            "Desmond", "Tiana",
            "Amari", "Teresa",
            "Jamal", "Carolyn",
            "Kody", "Jane",
            "Conor", "Jenny",
            "Quinton", "Maddison",
            "Beau", "Viviana",
            "Dorian", "Noelle",
            "Nelson", "Ashleigh",
            "Khalil", "Virginia",
            "Jaylin", "Dominique",
            "Elliott", "Jaqueline",
            "Demetrius", "Johanna",
            "Trevon", "Kaitlynn",
            "Noel", "Paulina",
            "Bobby", "Skye",
            "Billy", "India",
            "Brendon", "Ashanti",
            "Craig", "Lola",
            "Mekhi", "Amari",
            "Sam", "Anya",
            "Frederick", "Janelle",
            "Jessie", "Taryn",
            "Felipe", "Valentina",
            "Nathanael", "Harmony",
            "Roy", "Delilah",
            "Terrance", "Kristin",
            "Davion", "Janiya",
            "Kristian", "Estrella",
            "Nasir", "Macie",
            "Noe", "Kaleigh",
            "Jermaine", "Tamia",
            "Rene", "Madilyn",
            "Solomon", "Tania",
            "Jaylon", "Ayanna",
            "Franklin", "Alyson",
            "Bruce", "Elaina",
            "Reed", "Sarai",
            "Jarrett", "Jaelyn",
            "Reginald", "Carla",
            "Quincy", "Fernanda",
            "Ahmad", "Regan",
            "Zackery", "Alisha",
            "Jamari", "Ximena",
            "Kenny", "Destinee",
            "Moses", "Martha",
            "Grady", "Nataly",
            "Willie", "Marie",
            "Gerald", "Emilia",
            "Jaydon", "Cheyanne",
            "Allan", "Alanna",
            "Malcolm", "Haleigh",
            "Osvaldo", "Raegan",
            "Mohamed", "Ainsley",
            "Steve", "Kaelyn",
            "Terrell", "Nyla",
            "Alvin", "America",
            "Damion", "Jaiden",
            "Johnathon", "Miracle",
            "Jayce", "Athena",
            "Nehemiah", "Renee",
            "Anderson", "Amya",
            "Harley", "Alessandra",
            "Byron", "Ryan",
            "Triston", "Cara",
            "Kendall", "Samara",
            "Tomas", "Emilie",
            "Leonel", "Monique",
        };

        #endregion
        #region Lastnames

        readonly static string[] Lastnames = new[] {
        "Smith",
        "Johnson",
        "Williams",
        "Brown",
        "Jones",
        "Miller",
        "Davis",
        "Garcia",
        "Rodriguez",
        "Wilson",
        "Martinez",
        "Anderson",
        "Taylor",
        "Thomas",
        "Hernandez",
        "Moore",
        "Martin",
        "Jackson",
        "Thompson",
        "White",
        "Lopez",
        "Lee",
        "Gonzalez",
        "Harris",
        "Clark",
        "Lewis",
        "Robinson",
        "Walker",
        "Perez",
        "Hall",
        "Young",
        "Allen",
        "Sanchez",
        "Wright",
        "King",
        "Scott",
        "Green",
        "Baker",
        "Adams",
        "Nelson",
        "Hill",
        "Ramirez",
        "Campbell",
        "Mitchell",
        "Roberts",
        "Carter",
        "Phillips",
        "Evans",
        "Turner",
        "Torres",
        "Parker",
        "Collins",
        "Edwards",
        "Stewart",
        "Flores",
        "Morris",
        "Nguyen",
        "Murphy",
        "Rivera",
        "Cook",
        "Rogers",
        "Morgan",
        "Peterson",
        "Cooper",
        "Reed",
        "Bailey",
        "Bell",
        "Gomez",
        "Kelly",
        "Howard",
        "Ward",
        "Cox",
        "Diaz",
        "Richardson",
        "Wood",
        "Watson",
        "Brooks",
        "Bennett",
        "Gray",
        "James",
        "Reyes",
        "Cruz",
        "Hughes",
        "Price",
        "Myers",
        "Long",
        "Foster",
        "Sanders",
        "Ross",
        "Morales",
        "Powell",
        "Sullivan",
        "Russell",
        "Ortiz",
        "Jenkins",
        "Gutierrez",
        "Perry",
        "Butler",
        "Barnes",
        "Fisher",
        "Henderson",
        "Coleman",
        "Simmons",
        "Patterson",
        "Jordan",
        "Reynolds",
        "Hamilton",
        "Graham",
        "Kim",
        "Gonzales",
        "Alexander",
        "Ramos",
        "Wallace",
        "Griffin",
        "West",
        "Cole",
        "Hayes",
        "Chavez",
        "Gibson",
        "Bryant",
        "Ellis",
        "Stevens",
        "Murray",
        "Ford",
        "Marshall",
        "Owens",
        "Mcdonald",
        "Harrison",
        "Ruiz",
        "Kennedy",
        "Wells",
        "Alvarez",
        "Woods",
        "Mendoza",
        "Castillo",
        "Olson",
        "Webb",
        "Washington",
        "Tucker",
        "Freeman",
        "Burns",
        "Henry",
        "Vasquez",
        "Snyder",
        "Simpson",
        "Crawford",
        "Jimenez",
        "Porter",
        "Mason",
        "Shaw",
        "Gordon",
        "Wagner",
        "Hunter",
        "Romero",
        "Hicks",
        "Dixon",
        "Hunt",
        "Palmer",
        "Robertson",
        "Black",
        "Holmes",
        "Stone",
        "Meyer",
        "Boyd",
        "Mills",
        "Warren",
        "Fox",
        "Rose",
        "Rice",
        "Moreno",
        "Schmidt",
        "Patel",
        "Ferguson",
        "Nichols",
        "Herrera",
        "Medina",
        "Ryan",
        "Fernandez",
        "Weaver",
        "Daniels",
        "Stephens",
        "Gardner",
        "Payne",
        "Kelley",
        "Dunn",
        "Pierce",
        "Arnold",
        "Tran",
        "Spencer",
        "Peters",
        "Hawkins",
        "Grant",
        "Hansen",
        "Castro",
        "Hoffman",
        "Hart",
        "Elliott",
        "Cunningham",
        "Knight",
        "Bradley",
        "Carroll",
        "Hudson",
        "Duncan",
        "Armstrong",
        "Berry",
        "Andrews",
        "Johnston",
        "Ray",
        "Lane",
        "Riley",
        "Carpenter",
        "Perkins",
        "Aguilar",
        "Silva",
        "Richards",
        "Willis",
        "Matthews",
        "Chapman",
        "Lawrence",
        "Garza",
        "Vargas",
        "Watkins",
        "Wheeler",
        "Larson",
        "Carlson",
        "Harper",
        "George",
        "Greene",
        "Burke",
        "Guzman",
        "Morrison",
        "Munoz",
        "Jacobs",
        "Obrien",
        "Lawson",
        "Franklin",
        "Lynch",
        "Bishop",
        "Carr",
        "Salazar",
        "Austin",
        "Mendez",
        "Gilbert",
        "Jensen",
        "Williamson",
        "Montgomery",
        "Harvey",
        "Oliver",
        "Howell",
        "Dean",
        "Hanson",
        "Weber",
        "Garrett",
        "Sims",
        "Burton",
        "Fuller",
        "Soto",
        "Mccoy",
        "Welch",
        "Chen",
        "Schultz",
        "Walters",
        "Reid",
        "Fields",
        "Walsh",
        "Little",
        "Fowler",
        "Bowman",
        "Davidson",
        "May",
        "Day",
        "Schneider",
        "Newman",
        "Brewer",
        "Lucas",
        "Holland",
        "Wong",
        "Banks",
        "Santos",
        "Curtis",
        "Pearson",
        "Delgado",
        "Valdez",
        "Pena",
        "Rios",
        "Douglas",
        "Sandoval",
        "Barrett",
        "Hopkins",
        "Keller",
        "Guerrero",
        "Stanley",
        "Bates",
        "Alvarado",
        "Beck",
        "Ortega",
        "Wade",
        "Estrada",
        "Contreras",
        "Barnett",
        "Caldwell",
        "Santiago",
        "Lambert",
        "Powers",
        "Chambers",
        "Nunez",
        "Craig",
        "Leonard",
        "Lowe",
        "Rhodes",
        "Byrd",
        "Gregory",
        "Shelton",
        "Frazier",
        "Becker",
        "Maldonado",
        "Fleming",
        "Vega",
        "Sutton",
        "Cohen",
        "Jennings",
        "Parks",
        "Mcdaniel",
        "Watts",
        "Barker",
        "Norris",
        "Vaughn",
        "Vazquez",
        "Holt",
        "Schwartz",
        "Steele",
        "Benson",
        "Neal",
        "Dominguez",
        "Horton",
        "Terry",
        "Wolfe",
        "Hale",
        "Lyons",
        "Graves",
        "Haynes",
        "Miles",
        "Park",
        "Warner",
        "Padilla",
        "Bush",
        "Thornton",
        "Mccarthy",
        "Mann",
        "Zimmerman",
        "Erickson",
        "Fletcher",
        "Mckinney",
        "Page",
        "Dawson",
        "Joseph",
        "Marquez",
        "Reeves",
        "Klein",
        "Espinoza",
        "Baldwin",
        "Moran",
        "Love",
        "Robbins",
        "Higgins",
        "Ball",
        "Cortez",
        "Le",
        "Griffith",
        "Bowen",
        "Sharp",
        "Cummings",
        "Ramsey",
        "Hardy",
        "Swanson",
        "Barber",
        "Acosta",
        "Luna",
        "Chandler",
        "Blair",
        "Daniel",
        "Cross",
        "Simon",
        "Dennis",
        "Oconnor",
        "Quinn",
        "Gross",
        "Navarro",
        "Moss",
        "Fitzgerald",
        "Doyle",
        "Mclaughlin",
        "Rojas",
        "Rodgers",
        "Stevenson",
        "Singh",
        "Yang",
        "Figueroa",
        "Harmon",
        "Newton",
        "Paul",
        "Manning",
        "Garner",
        "Mcgee",
        "Reese",
        "Francis",
        "Burgess",
        "Adkins",
        "Goodman",
        "Curry",
        "Brady",
        "Christensen",
        "Potter",
        "Walton",
        "Goodwin",
        "Mullins",
        "Molina",
        "Webster",
        "Fischer",
        "Campos",
        "Avila",
        "Sherman",
        "Todd",
        "Chang",
        "Blake",
        "Malone",
        "Wolf",
        "Hodges",
        "Juarez",
        "Gill",
        "Farmer",
        "Hines",
        "Gallagher",
        "Duran",
        "Hubbard",
        "Cannon",
        "Miranda",
        "Wang",
        "Saunders",
        "Tate",
        "Mack",
        "Hammond",
        "Carrillo",
        "Townsend",
        "Wise",
        "Ingram",
        "Barton",
        "Mejia",
        "Ayala",
        "Schroeder",
        "Hampton",
        "Rowe",
        "Parsons",
        "Frank",
        "Waters",
        "Strickland",
        "Osborne",
        "Maxwell",
        "Chan",
        "Deleon",
        "Norman",
        "Harrington",
        "Casey",
        "Patton",
        "Logan",
        "Bowers",
        "Mueller",
        "Glover",
        "Floyd",
        "Hartman",
        "Buchanan",
        "Cobb",
        "French",
        "Kramer",
        "Mccormick",
        "Clarke",
        "Tyler",
        "Gibbs",
        "Moody",
        "Conner",
        "Sparks",
        "Mcguire",
        "Leon",
        "Bauer",
        "Norton",
        "Pope",
        "Flynn",
        "Hogan",
        "Robles",
        "Salinas",
        "Yates",
        "Lindsey",
        "Lloyd",
        "Marsh",
        "Mcbride",
        "Owen",
        "Solis",
        "Pham",
        "Lang",
        "Pratt",
        "Lara",
        "Brock",
        "Ballard",
        "Trujillo",
        "Shaffer",
        "Drake",
        "Roman",
        "Aguirre",
        "Morton",
        "Stokes",
        "Lamb",
        "Pacheco",
        "Patrick",
        "Cochran",
        "Shepherd",
        "Cain",
        "Burnett",
        "Hess",
        "Li",
        "Cervantes",
        "Olsen",
        "Briggs",
        "Ochoa",
        "Cabrera",
        "Velasquez",
        "Montoya",
        "Roth",
        "Meyers",
        "Cardenas",
        "Fuentes",
        "Weiss",
        "Hoover",
        "Wilkins",
        "Nicholson",
        "Underwood",
        "Short",
        "Carson",
        "Morrow",
        "Colon",
        "Holloway",
        "Summers",
        "Bryan",
        "Petersen",
        "Mckenzie",
        "Serrano",
        "Wilcox",
        "Carey",
        "Clayton",
        "Poole",
        "Calderon",
        "Gallegos",
        "Greer",
        "Rivas",
        "Guerra",
        "Decker",
        "Collier",
        "Wall",
        "Whitaker",
        "Bass",
        "Flowers",
        "Davenport",
        "Conley",
        "Houston",
        "Huff",
        "Copeland",
        "Hood",
        "Monroe",
        "Massey",
        "Roberson",
        "Combs",
        "Franco",
        "Larsen",
        "Pittman",
        "Randall",
        "Skinner",
        "Wilkinson",
        "Kirby",
        "Cameron",
        "Bridges",
        "Anthony",
        "Richard",
        "Kirk",
        "Bruce",
        "Singleton",
        "Mathis",
        "Bradford",
        "Boone",
        "Abbott",
        "Charles",
        "Allison",
        "Sweeney",
        "Atkinson",
        "Horn",
        "Jefferson",
        "Rosales",
        "York",
        "Christian",
        "Phelps",
        "Farrell",
        "Castaneda",
        "Nash",
        "Dickerson",
        "Bond",
        "Wyatt",
        "Foley",
        "Chase",
        "Gates",
        "Vincent",
        "Mathews",
        "Hodge",
        "Garrison",
        "Trevino",
        "Villarreal",
        "Heath",
        "Dalton",
        "Valencia",
        "Callahan",
        "Hensley",
        "Atkins",
        "Huffman",
        "Roy",
        "Boyer",
        "Shields",
        "Lin",
        "Hancock",
        "Grimes",
        "Glenn",
        "Cline",
        "Delacruz",
        "Camacho",
        "Dillon",
        "Parrish",
        "Oneill",
        "Melton",
        "Booth",
        "Kane",
        "Berg",
        "Harrell",
        "Pitts",
        "Savage",
        "Wiggins",
        "Brennan",
        "Salas",
        "Marks",
        "Russo",
        "Sawyer",
        "Baxter",
        "Golden",
        "Hutchinson",
        "Liu",
        "Walter",
        "Mcdowell",
        "Wiley",
        "Rich",
        "Humphrey",
        "Johns",
        "Koch",
        "Suarez",
        "Hobbs",
        "Beard",
        "Gilmore",
        "Ibarra",
        "Keith",
        "Macias",
        "Khan",
        "Andrade",
        "Ware",
        "Stephenson",
        "Henson",
        "Wilkerson",
        "Dyer",
        "Mcclure",
        "Blackwell",
        "Mercado",
        "Tanner",
        "Eaton",
        "Clay",
        "Barron",
        "Beasley",
        "Oneal",
        "Preston",
        "Small",
        "Wu",
        "Zamora",
        "Macdonald",
        "Vance",
        "Snow",
        "Mcclain",
        "Stafford",
        "Orozco",
        "Barry",
        "English",
        "Shannon",
        "Kline",
        "Jacobson",
        "Woodard",
        "Huang",
        "Kemp",
        "Mosley",
        "Prince",
        "Merritt",
        "Hurst",
        "Villanueva",
        "Roach",
        "Nolan",
        "Lam",
        "Yoder",
        "Mccullough",
        "Lester",
        "Santana",
        "Valenzuela",
        "Winters",
        "Barrera",
        "Leach",
        "Orr",
        "Berger",
        "Mckee",
        "Strong",
        "Conway",
        "Stein",
        "Whitehead",
        "Bullock",
        "Escobar",
        "Knox",
        "Meadows",
        "Solomon",
        "Velez",
        "Odonnell",
        "Kerr",
        "Stout",
        "Blankenship",
        "Browning",
        "Kent",
        "Lozano",
        "Bartlett",
        "Pruitt",
        "Buck",
        "Barr",
        "Gaines",
        "Durham",
        "Gentry",
        "Mcintyre",
        "Sloan",
        "Melendez",
        "Rocha",
        "Herman",
        "Sexton",
        "Moon",
        "Hendricks",
        "Rangel",
        "Stark",
        "Lowery",
        "Hardin",
        "Hull",
        "Sellers",
        "Ellison",
        "Calhoun",
        "Gillespie",
        "Mora",
        "Knapp",
        "Mccall",
        "Morse",
        "Dorsey",
        "Weeks",
        "Nielsen",
        "Livingston",
        "Leblanc",
        "Mclean",
        "Bradshaw",
        "Glass",
        "Middleton",
        "Buckley",
        "Schaefer",
        "Frost",
        "Howe",
        "House",
        "Mcintosh",
        "Ho",
        "Pennington",
        "Reilly",
        "Hebert",
        "Mcfarland",
        "Hickman",
        "Noble",
        "Spears",
        "Conrad",
        "Arias",
        "Galvan",
        "Velazquez",
        "Huynh",
        "Frederick",
        "Randolph",
        "Cantu",
        "Fitzpatrick",
        "Mahoney",
        "Peck",
        "Villa",
        "Michael",
        "Donovan",
        "Mcconnell",
        "Walls",
        "Boyle",
        "Mayer",
        "Zuniga",
        "Giles",
        "Pineda",
        "Pace",
        "Hurley",
        "Mays",
        "Mcmillan",
        "Crosby",
        "Ayers",
        "Case",
        "Bentley",
        "Shepard",
        "Everett",
        "Pugh",
        "David",
        "Mcmahon",
        "Dunlap",
        "Bender",
        "Hahn",
        "Harding",
        "Acevedo",
        "Raymond",
        "Blackburn",
        "Duffy",
        "Landry",
        "Dougherty",
        "Bautista",
        "Shah",
        "Potts",
        "Arroyo",
        "Valentine",
        "Meza",
        "Gould",
        "Vaughan",
        "Fry",
        "Rush",
        "Avery",
        "Herring",
        "Dodson",
        "Clements",
        "Sampson",
        "Tapia",
        "Bean",
        "Lynn",
        "Crane",
        "Farley",
        "Cisneros",
        "Benton",
        "Ashley",
        "Mckay",
        "Finley",
        "Best",
        "Blevins",
        "Friedman",
        "Moses",
        "Sosa",
        "Blanchard",
        "Huber",
        "Frye",
        "Krueger",
        "Bernard",
        "Rosario",
        "Rubio",
        "Mullen",
        "Benjamin",
        "Haley",
        "Chung",
        "Moyer",
        "Choi",
        "Horne",
        "Yu",
        "Woodward",
        "Ali",
        "Nixon",
        "Hayden",
        "Rivers",
        "Estes",
        "Mccarty",
        "Richmond",
        "Stuart",
        "Maynard",
        "Brandt",
        "Oconnell",
        "Hanna",
        "Sanford",
        "Sheppard",
        "Church",
        "Burch",
        "Levy",
        "Rasmussen",
        "Coffey",
        "Ponce",
        "Faulkner",
        "Donaldson",
        "Schmitt",
        "Novak",
        "Costa",
        "Montes",
        "Booker",
        "Cordova",
        "Waller",
        "Arellano",
        "Maddox",
        "Mata",
        "Bonilla",
        "Stanton",
        "Compton",
        "Kaufman",
        "Dudley",
        "Mcpherson",
        "Beltran",
        "Dickson",
        "Mccann",
        "Villegas",
        "Proctor",
        "Hester",
        "Cantrell",
        "Daugherty",
        "Cherry",
        "Bray",
        "Davila",
        "Rowland",
        "Levine",
        "Madden",
        "Spence",
        "Good",
        "Irwin",
        "Werner",
        "Krause",
        "Petty",
        "Whitney",
        "Baird",
        "Hooper",
        "Pollard",
        "Zavala",
        "Jarvis",
        "Holden",
        "Haas",
        "Hendrix",
        "Mcgrath",
        "Bird",
        "Lucero",
        "Terrell",
        "Riggs",
        "Joyce",
        "Mercer",
        "Rollins",
        "Galloway",
        "Duke",
        "Odom",
        "Andersen",
        "Downs",
        "Hatfield",
        "Benitez",
        "Archer",
        "Huerta",
        "Travis",
        "Mcneil",
        "Hinton",
        "Zhang",
        "Hays",
        "Mayo",
        "Fritz",
        "Branch",
        "Mooney",
        "Ewing",
        "Ritter",
        "Esparza",
        "Frey",
        "Braun",
        "Gay",
        "Riddle",
        "Haney",
        "Kaiser",
        "Holder",
        "Chaney",
        "Mcknight",
        "Gamble",
        "Vang",
        "Cooley",
        "Carney",
        "Cowan",
        "Forbes",
        "Ferrell",
        "Davies",
        "Barajas",
        "Shea",
        "Osborn",
        "Bright",
        "Cuevas",
        "Bolton",
        "Murillo",
        "Lutz",
        "Duarte",
        "Kidd",
        "Key",
        "Cooke",
        };

        #endregion
		
		
//        readonly static int[] WishlistSize = new [] { 0, 0, 0, 1, 2, 3, 4 };

        public IUserRepository Users { get; set; }
        public IGroupRepository Groups { get; set; }
        public IAccountingService Accounting { get; set; }
        public IAwardRepository Awards { get; set; }
        public IUserAwardRepository UserAwards { get; set; }
        public IQuizRepository Quizzes { get; set; }
        public IQuizResultRepository QuizResults { get; set; }
        public ICatalogService Catalog { get; set; }
        public IOrderRepository Orders { get; set; }

        [HttpGet]
        public virtual ActionResult Randomizer()
        {
            return View(new RandomizerModel {
                From = DateTime.UtcNow.AddYears(-1),
                To = DateTime.UtcNow,
            });
        }

        [HttpPost]
        public virtual ActionResult Randomizer(RandomizerModel model)
        {
            // keep all the users in a local structure
            var r = new Random();
            var all = new Dictionary<string, User>();
            var execs = new List<User>();
            var managers = new List<User>();
            var groups = Groups.All().WithDocuments().ToList();

            // load up all the quizes and awards that we could do
            var quizzes = Quizzes.All().WithDocuments();
			// var awards = Awards.All().WithDocuments();

            // load up a random slice of about 10% of all products
            var products =
                Catalog.GetCategories(Application.DefaultCatalog)
                       .Products
                       .Where(x => r.NextDouble() < 0.10)
                       .ToArray();
            var averageprice = (int)products.Average(x => x.Price);

            var start = (model.From ?? DateTime.Now.AddYears(-1));
            var end = (model.To ?? DateTime.Now);
            var days = (int)(end-start).TotalDays;

            var dobstart = new DateTime(1955, 1, 1);
            const int dobdays = 365*35;

            for (var n=all.Count; n<model.Users; n++)
            {
                // create basic user properties
                var first = Firstnames[(int)(Math.Pow(r.NextDouble(), r.NextDouble()) * Firstnames.Length)];
                var last = Lastnames[(int)(Math.Pow(r.NextDouble(), r.NextDouble()) * Lastnames.Length)];
                var login = (first[0] + last).ToSlug();
                var u = new User {
                  Document = new Document { Id = Document.For<User>(login) },
                  Login = login,
                  FirstName = first,
                  LastName = last,
                  Email = String.Format("{0}.{1}@{2}", first.ToSlug(), last.ToSlug(), model.EmailDomain),
                  State = UserState.Active,
                  DateBirth = dobstart.AddDays(r.Next(1, dobdays)),
                  DateHired = end.AddDays(-1 * r.Next(1, 3000)),
                  DateActivated = start.AddDays(r.Next(0, days)),
                };
                u.SetPassword("asdf");

                // place into group/management hierarchy.. execs in charge of a group, with
                // several managers in each group with employers reporting to them.
                if (groups.Count > 2)
                {
                    if (execs.Count < groups.Count)
                    {
                        // there's 1 exec for each group
                        var g = groups[execs.Count];
                        u.Group = g.Document.Id;
                        u.ManagedGroups = new[] { u.Group };
                        u.Title = "VP of " + g.Name;
                        execs.Add(u);

                        // execs get a large budget
                        Accounting.GetBudgetLedger(u, true,
                            new Budget {
                                RefreshLimit = 3000,
                                RefreshInterval = BudgetRefreshInterval.Monthly,
                            });
                    }
                    else if (managers.Count < 3 * groups.Count)
                    {
                        // there are a few managers in each group        
                        u.Group = groups[r.Next(0, groups.Count - 1)].Document.Id;
                        u.Manager = execs.Where(x => x.Group == u.Group).First().Document.Id;
                        u.Title = "Supervisor";
                        managers.Add(u);

                        // managers get a modest budget
                        Accounting.GetBudgetLedger(u, true,
                            new Budget {
                                RefreshLimit = r.NextDouble() < 0.50 ? 1500 : 2000,
                                RefreshInterval = BudgetRefreshInterval.Monthly,
                            });                    
                    }
                    else
                    {
                        // assign the user to a random manager (and place in his group)
                        var m = managers[r.Next(0, managers.Count-1)];
                        u.Manager = m.Document.Id;
                        u.Group = m.Group;
                    }
                }
                
                try
                {
                    Users.Save(u);
                    all.Add(u.Document.Id, u);
                }
                catch
                {
                }
            }

            // refresh budgets every day
            for (var n=0; n<days; n++)
            {
                Accounting.RefreshBudgets(start.AddDays(days));
            }

            foreach (var u in all.Values)
            {
                // some users just don't do anything
                var active = r.NextDouble() < 0.95;
                if (active)
                {
                    // avg number of days between logins
                    var frequency = r.Next(7, 60);     
                    var d = start;
                    while (true)
                    {
                        // users are fairly predictable, within about 25% tolerance on either side
                        d = d.AddDays(r.Next(
                            (int)(frequency - (frequency * 0.25)),
                            (int)(frequency + (frequency * 0.25))
                        ));
                        if (d > end) break;

                        // lets meet the bobs
                        if (r.NextDouble() < 0.05)
                        {
                            u.State = UserState.Terminated;
                            u.DateTerminated = d;
                            u.StateChanged = d; 
                            break;
                        }

                        // usually they log in because they got some points
                        var ledger = Accounting.GetPointsLedger(u);
                        if (r.NextDouble() < 0.60)
                        {
                            Accounting.CreateTransaction(
                                Application.GeneralControlAccount,
                                ledger.Account.Document.Id,
                                r.Next(averageprice/10, averageprice/2),
                                "Congrulations on your sale!",
                                "Sales",
                                d.AddHours(-1 * r.Next(2, 72)),
                                null);
                        }

                        // sometimes we forget our password
                        if (r.NextDouble() < 0.05)
                        {
                            u.IncrementLogins(false, Request, d);

                            // and we try again
                            if (r.NextDouble() < 0.75)
                            {
                                d = d.AddMinutes(1);
                                u.IncrementLogins(false, Request, d);
                            }
                           
                            // and again
                            if (r.NextDouble() < 0.75)
                            {
                                d = d.AddMinutes(1);
                                u.IncrementLogins(false, Request, d);
                            }

                            // and again, and /again/
                            // the time for honoring yourself will soon be over

                            // but then we usually reset it and get logged in
                            if (r.NextDouble() < 0.80)
                            {
                                d = d.AddMinutes(5);
                            }
                            else
                            {
                                break;
                            }
                        }
                        
                        // login!
                        u.IncrementLogins(true, Request, d);

                        // if this is their first login, make sure they accept the terms of service
                        if (!u.DateAcceptedTermsOfService.HasValue)
                        {
                            u.DateAcceptedTermsOfService = d;
                        }

                        // try taking a quiz!
                        if (r.NextDouble() < 0.20)
                        {
                            foreach (var q in quizzes)
                            {
                                if (q.CanUserTakeQuiz(u, d, QuizResults.GetResults(q, u)).Available)
                                {
                                    // most quizzes are easy
                                    var qr = q.Score(
                                        u, 
                                        q.Questions.Select(x => {
                                            var correct = (r.NextDouble() < 0.95);
                                            return x.Answers.IndexOf(x.Answers.First(y => y.Correct == correct));
                                        }).ToArray()
                                    );
                                    qr.Taken = d.AddMinutes(5);
                                    if (qr.Passed && qr.PointsEarned.HasValue && qr.PointsEarned > 0)
                                    {
                                        var tx = Accounting.CreateProgramAward(q, null, u, qr.PointsEarned.Value, q.Content.Title);
                                        qr.Transaction = tx.Document.Id;
                                    }
                                    QuizResults.Save(qr);
                                    break;
                                }
                            }
                        }

                        // send an award
                        if (r.NextDouble() < 0.10)
                        {
                        }

                        // if we have at least enough points to buy something 'average', try to place an order
                        if (ledger.Balance > averageprice)
                        {
                            var cart = new List<CatalogProduct>();
                            while (true)
                            {
                                var p = products[r.Next(0, products.Length-1)];
                                if (cart.Sum(x => x.Price) + p.Price < ledger.Balance)
                                {
                                    cart.Add(p);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (cart.Count > 0)
                            {
                                var o = new Order
                                {   
                                    User = u.Document.Id,
                                    State = OrderStateMachine.Create(),
                                    ShippingName = u.DisplayName,
                                    ShippingAddress = new Address { Address1 = "123 Main St.", City = "Minneapolis", State = "MN", PostalCode = "55456" },
                                    ShippingPhone = new Phone { Number = "(952)555-7834" },
                                    Items = cart.Select(x => new OrderItem
                                    {
                                        ProductId = x.Id,
                                        ProductName = x.Name,
                                        OptionName = x.Options[0].Name,
                                        OptionSku =  x.Options[1].Sku,
                                        Description = x.Description,
                                        UnitPrice = x.Price,
                                        Quantity = 1,
                                        Stock = x.Stock,
                                        State = OrderItemStateMachine.Create(),
                                    }).ToArray(),
                                };
                                o.State[0].Changed = d.AddMinutes(15);
                                var tx = Accounting.CreateOrderPayment(u, o);
                                o.Transaction = tx.Document.Id;
                                Orders.Save(o);
                            }
                        }

                        // distribute budget
                        var budgetLedger = Accounting.GetBudgetLedger(u);
                        if (null != budgetLedger &&
                            null != budgetLedger.Account &&
                            null != budgetLedger.Account.Budget &&
                            r.NextDouble() < 0.50)
                        {
                            var budgetBalance = budgetLedger.Balance;
                            if (budgetBalance > budgetLedger.Account.Budget.RefreshLimit.Value * 0.15)
                            {
                                // pick a random user that reports to us
                                var reports = all.Values.Where(x => x.Manager == u.Document.Id).ToList();
                                if (reports.Count > 0)
                                {
                                    var awardee = reports[r.Next(0, reports.Count-1)];
                                    Accounting.CreateBudgetTransfer(
                                        u,
                                        awardee,
                                        r.Next((int)(budgetBalance * 0.10), (int)(budgetBalance * 0.85)),
                                        "Budget Award");
                                }
                            }
                        }

                    }
                }
                Users.Save(u);
            }
            
            Notifier.Notify(Severity.Success, "Randomized!", "Random activity succesfully created. You may want to compact the database.", null);
            return RedirectToAction(MVC.Admin.User.Index());
        }

        #endregion
    }
}
