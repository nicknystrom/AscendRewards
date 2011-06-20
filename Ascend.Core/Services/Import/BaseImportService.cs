using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RedBranch.Hammock;

namespace Ascend.Core.Services.Import
{
    public enum ImportDataType
    {
        String,  // system.string
        Number,  // system.decimal?
        Date,    // system.datetime?
        Boolean  // system.bool?
    }

    public enum ImportStep
    {
        BuildLayout,
        ValidateLayout,
        ConvertRowData,
        FindRowEntities,
        CreateRowEntities,
        ValidateRows,
        ImportRows,
        SaveRows,
        Complete,
    }

    public class ImportResult
    {
        public DateTime Date { get; set; }
        public string File { get; set; }
        public string Type { get; set; }

        public bool Success { get; set; }
        public ImportStep Step { get; set; }
        public List<string> Problems { get; set; }
        public string[] Layout { get; set; }
        public List<ImportResultRow> Rows { get; set; }

        public ImportResult() { Success = true; }

        public ImportResult(string problem)
        {
            AddProblem(problem);
        }

        public void AddProblem(string problem)
        {
            if (null == Problems) Problems = new List<string>(1);
            Problems.Add(problem);
            Success = false;
        }

        private static readonly ImportResult _ok = new ImportResult();
        public static ImportResult Ok { get { return _ok; } } 

        public class ImportResultRow
        {
            public bool IsNew { get; set; }
            public bool IsValid { get; set; }
            public object[] Data { get; set; }
            public IList<string> Problems { get; set; }
        }

        public ImportAttempt ToAttempt()
        {
            return new ImportAttempt
            {
                Date          = Date,
                RowsFailed    = null == Rows ? 0 : Rows.Count(x => null != x.Problems && x.Problems.Count  > 0),
                RowsProcessed = null == Rows ? 0 : Rows.Count(x => null == x.Problems || x.Problems.Count == 0),
                Step          = Step,
                Success       = Success,
                Problems      = (Problems ?? new List<string>()).Concat(
                                (Rows ?? new List<ImportResultRow>())
                                .Where(x => x.Problems != null && x.Problems.Count > 0)
                                .SelectMany(x => x.Problems))
                                .Take(10).ToList(),
            };
        }
    }

    public interface IImportSource : IEnumerable<object[]>
    {
        string[] Fields { get; }
        int? Rows { get;  }
    }

    public interface IImportService<TEntity> where TEntity : Entity
    {
        ImportResult Import(Core.Import import, IImportSource source, bool save);
    }

    public abstract class BaseImportService<TEntity, TEnum> : IImportService<TEntity> where TEntity : Entity
    {
        public class ImportRow
        {
            public bool IsNew { get; set; }
            public TEntity Entity { get; set; }
            public object[] Values { get; set; }

            public ImportDataType[] Types { get; set; }
            public TEnum[] Layout { get; set; }
            
            public IList<string> Problems { get; set; }
            public bool IsValid { get { return (null == Problems || Problems.Count == 0); } }

            public void AddProblem(string problem)
            {
                if (null == Problems)
                {
                    Problems = new List<string>();
                }
                Problems.Add(problem);
            }

            public bool Has(TEnum key)
            {
                return Layout.Contains(key);
            }

            public object this[TEnum key]
            {
                get
                {
                    var i = Array.IndexOf(Layout, key);
                    if (i >= 0)
                    {
                        return Values[i];
                    }
                    return null;
                }
            }

            public ImportRow(object[] data, ImportDataType[] types, TEnum[] layout)
            {
                Types = types;
                Layout = layout;
                Values = new object[types.Length];
                for (int i=0; i<types.Length; i++)
                {
                    var x = data[i];
                    if (null == x)
                    {
                        Values[i] = null;
                        continue;
                    }

                    try
                    {
                        switch (types[i])
                        {
                            case ImportDataType.Boolean:
                                if (x is bool)
                                {
                                    Values[i] = x;
                                }
                                else if (x is Int32)
                                {
                                    Values[i] = 0 != (int) x;
                                }
                                else if (x is string)
                                {
                                    var a = (string) x;
                                    Values[i] = String.Equals("t", a, StringComparison.InvariantCultureIgnoreCase) ||
                                                String.Equals("true", a, StringComparison.InvariantCultureIgnoreCase) ||
                                                String.Equals("y", a, StringComparison.InvariantCultureIgnoreCase) ||
                                                String.Equals("yes", a, StringComparison.InvariantCultureIgnoreCase) ||
                                                String.Equals("1", a, StringComparison.InvariantCultureIgnoreCase);
                                }
                                continue;

                            case ImportDataType.String:
                                Values[i] = x.ToString();
                                continue;

                            case ImportDataType.Date:
                                if (x is DateTime)
                                {
                                    Values[i] = (DateTime?)(DateTime)x;
                                    continue;
                                }
                                if (x is string)
                                {
                                    var a = (string)x;
                                    if (String.IsNullOrEmpty(a) || "-" == a)
                                    {
                                        Values[i] = null;
                                    }
                                    else
                                    {
                                        Values[i] = (DateTime?)DateTime.Parse(a);
                                    }
                                }
                                continue;

                            case ImportDataType.Number:
                                if (x is int)
                                {
                                    Values[i] = (decimal?)(int)x;
                                }
                                else if (x is double)
                                {
                                    Values[i] = (decimal?)(double)x;
                                }
                                else if (x is decimal)
                                {
                                    Values[i] = (decimal?)x;
                                }
                                else if (x is string)
                                {
                                    var a = ((string)x).Trim();
                                    if (String.IsNullOrEmpty(a))
                                    {
                                        continue;
                                    }
                                    if (a.StartsWith("$"))
                                    {
                                        a = a.Substring(1);
                                    }
                                    if (a != "-" && a.Length > 0)
                                    {
                                        decimal d;
                                        float f;
                                        if (decimal.TryParse(a, out d))
                                        {
                                            Values[i] = (decimal?) d;
                                        }
                                        else if (float.TryParse(a, out f))
                                        {
                                            // handles values encoded with scientific notation, typically because the
                                            // underlying excel value was 7.99999999999999999999. FDIV anyone?
                                            Values[i] = (decimal?) f;
                                        }
                                        else
                                        {
                                            throw new Exception("Invalid numeric format, '" + a + "'.");    
                                        }
                                    }
                                }
                                continue;
                        }
                    }
                    catch
                    {
                        AddProblem(String.Format(
                            "Could not convert value '{0}' to type {1} in column {2}.", 
                            Values[i], 
                            Types[i], 
                            Layout[i]
                        ));
                    }
                }
            }
        }

        void FillRowResults(ImportResult result, List<ImportRow> rows)
        {
            result.Rows = rows.Select(x => new ImportResult.ImportResultRow { 
                                            IsNew = x.IsNew,
                                            IsValid = x.IsValid,
                                            Data = x.Values,
                                            Problems = x.Problems, }).ToList();
        }

        public ImportResult Import(Core.Import import, IImportSource source, bool save)
        {
            var result = new ImportResult
            {
                Date = DateTime.UtcNow,
                File = import.Location,
                Type = import.Type.ToString()
            };
            try
            {
                // build columns
                result.Step = ImportStep.BuildLayout;
                var layout = import.Columns.Select(x =>
                        String.IsNullOrEmpty(x.Target)
                            ? (TEnum)Enum.Parse(typeof(TEnum), "None")
                            : (TEnum)Enum.Parse(typeof(TEnum), x.Target)
                        ).ToArray();
                result.Layout = layout.Select(x => x.ToString()).ToArray();

                // get data types
                var types = layout.Select(x => GetColumnType(x)).ToArray();
                
                // validate layout
                result.Step = ImportStep.ValidateLayout;
                var layoutValid = ValidateLayout(layout);
                if (!layoutValid.Success)
                {
                    layoutValid.Problems.Each(x => result.AddProblem(x));
                    return result;
                }

                // convert all row data
                result.Step = ImportStep.ConvertRowData;
                var rows = source.Select(data => new ImportRow(data, types, layout)).ToList();
                if (rows.Any(x => !x.IsValid))
                {
                    FillRowResults(result, rows);
                    result.AddProblem("Some data could not be converted to the correct format. See row-by-row results for specific problems.");
                    return result;
                }

                // find existing rows
                result.Step = ImportStep.FindRowEntities;
                Parallel.ForEach(rows, r => r.Entity = Find(r));
                
                // create missing rows
                result.Step = ImportStep.CreateRowEntities;
                Parallel.ForEach(
                    rows.Where(x => null == x.Entity),
                    r => {
                            r.Entity = Create(r);
                            r.IsNew = true;
                        });

                // validate all row data
                result.Step = ImportStep.ValidateRows;
                foreach (var r in rows)
                {
                    ValidateRow(r);
                }

                // apply all row values
                result.Step = ImportStep.ImportRows;
                Parallel.ForEach(rows, r => Apply(r.Entity, r));
                if (rows.Any(x => !x.IsValid))
                {
                    FillRowResults(result, rows);
                    result.AddProblem("Some data could not be imported into the system. See row-by-row results for specific problems.");
                    return result;
                }

                // save all records  
                if (save)
                {
                    result.Step = ImportStep.SaveRows;
                    Parallel.ForEach(rows, r =>
                    {
                        try
                        {
                            Save(r.Entity);
                        }
                        catch (CouchException ex)
                        {
                            if (ex.Status == (int) HttpStatusCode.Conflict)
                            {
                                // conflicts typically occur when a unique id is supplied for two or more 
                                // new rows in an import sheet.. i.e. product id 1234 appears twice, and is new
                                r.AddProblem(
                                    @"There was a conflict saving this record, typically because a unique was duplicated in the spreadsheet.");
                            }
                            else
                            {
                                throw;
                            }
                        }

                    });
                }

                Completed();

                FillRowResults(result, rows);
                result.Step = ImportStep.Complete;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.AddProblem(String.Format("ERROR: Import encountered an unexpected error, proces aborted.<br/>Error: {0}<br />Stack: {1}.", (ex.InnerException ?? ex).Message, (ex.InnerException ?? ex).StackTrace));
                if (result.Step == ImportStep.SaveRows)
                {
                    result.AddProblem("DANGER: An error occurred DURING the Save step, meaning that some rows may have been written to the database, and some not. Re-importing this sheet could cause duplication of data.");
                }
            }
            return result;
        }

        public abstract ImportResult ValidateLayout(TEnum[] layout);
        public abstract void ValidateRow(ImportRow row);
        public abstract TEntity Find(ImportRow row);
        public abstract TEntity Create(ImportRow row);
        public abstract ImportDataType GetColumnType(TEnum x);
        public abstract void Apply(TEntity entity, ImportRow row);
        public abstract void Save(TEntity entity);

        public virtual void Completed()
        {
            // optional extension point for subclass
        }
    }
}