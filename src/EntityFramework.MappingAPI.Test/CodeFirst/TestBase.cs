using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace EntityFramework.MappingAPI.Test.CodeFirst
{
    [TestFixture]
    public abstract class TestBase
    {
        [SetUp]
        public virtual void Setup()
        {
            //Database.SetInitializer(new CreateDatabaseIfNotExists<TestContext>());
            Database.SetInitializer<TestContext>(null);
        }

        protected TestContext GetContext()
        {
            var ctx = new TestContext();

            ctx.Configuration.AutoDetectChangesEnabled = false;
            ctx.Configuration.LazyLoadingEnabled = false;
            ctx.Configuration.ProxyCreationEnabled = false;
            ctx.Configuration.ValidateOnSaveEnabled = false;

            return ctx;
        }

        protected void InitializeContext()
        {
            using (var ctx = GetContext())
            {
                var sw = new Stopwatch();
                sw.Start();
                var tmp = ctx.Pages.Count();
                sw.Stop();
                Console.WriteLine("Initializing dbmodel took: {0}ms", sw.Elapsed.TotalMilliseconds);
            }
        }
    }
}