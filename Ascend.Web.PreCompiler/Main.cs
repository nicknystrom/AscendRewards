using System;
using System.Collections.Generic;
using System.IO;

using Spark.Web.Mvc;
using Spark;

namespace Ascend.Web.PreCompiler
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            var location = typeof(MvcApplication).Assembly.Location;
            string target = Path.ChangeExtension(location, ".Views.dll");
            string root = Path.GetDirectoryName(Path.GetDirectoryName(location));
            //string webBinPath = Path.Combine (directoryName, "bin");
            
            Console.WriteLine("Precompiling to target: {0}", target);
            Console.WriteLine("Root folder: {0}", root);
   
            try
            {            
                var factory = new SparkViewFactory(MvcApplication.CreateSparkSettings());
                factory.ViewFolder = new VirtualPathCompatableViewFolder(root);
                factory.Engine.ViewFolder = factory.ViewFolder;
                
                var builder = new DefaultDescriptorBuilder(factory.Engine);
                builder.Filters.Add(new Ascend.Web.AreaDescriptorFilter());
                factory.DescriptorBuilder = builder;
                
                var batch = new SparkBatchDescriptor(target);
                batch.FromAssembly(typeof(MvcApplication).Assembly);
                
                var descriptors = new List<SparkViewDescriptor>();
                foreach (var entry in batch.Entries)
                {
                    var t = entry.ControllerType;
                    var name = t.Name.Substring(0, t.Name.Length - ("Controller".Length));
                    var parts = t.Namespace.Split('.');
                    var area = parts[1 + Array.LastIndexOf(parts, "Areas")];
                    
                    foreach (var view in factory.ViewFolder.ListViews(Path.Combine(root, "Areas", area, "Views", name)))
                    {
                        var locations = new List<string>();
                        var descriptor = factory.DescriptorBuilder.BuildDescriptor(
                            new BuildDescriptorParams(
                                t.Namespace,
                                name,
                                Path.GetFileNameWithoutExtension(view),
                                null,
                                true,
                                new Dictionary<string, object>  { { "area", area } }),
                            locations);
                        descriptors.Add(descriptor);
                    }
                }
                
                var result = factory.Engine.BatchCompilation(target, descriptors);
                
                // var descriptors = factory.CreateDescriptors(batch);
                // factory.Precompile(batch);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }
    }
}
