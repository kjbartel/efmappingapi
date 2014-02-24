using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EntityFramework.MappingAPI.Extensions;
using EntityFramework.MappingAPI.Test.CodeFirst.Domain;
using NUnit.Framework;

namespace EntityFramework.MappingAPI.Test.CodeFirst
{
    public static class TestHelper
    {
        public static IColumnMapping HasColumnName(this IColumnMapping c, string columnName)
        {
            string message = string.Format("Property {0} should be mapped to col {1}, but was mapped to {2}", c.PropertyName, columnName, c.ColumnName);
            Assert.AreEqual(columnName, c.ColumnName, message);
            return c;
        }

        public static IColumnMapping IsPk(this IColumnMapping c, bool isPk = true)
        {
            string message = string.Format("Property {0} pk flag should be {1}, but was {2}", c.PropertyName, isPk, c.IsPk);
            Assert.AreEqual(isPk, c.IsPk, message);
            return c;
        }

        public static IColumnMapping IsFk(this IColumnMapping c, bool isFk = true)
        {
            string message = string.Format("Property {0} fk flag should be {1}, but was {2}", c.PropertyName, isFk, c.IsFk);
            Assert.AreEqual(isFk, c.IsFk, message);
            return c;
        }

        public static IColumnMapping IsNavigationProperty(this IColumnMapping c, bool isNavProp = true)
        {
            string message = string.Format("Property {0} navigationProperty flag should be {1}, but was {2}", c.PropertyName, isNavProp, c.IsNavigationProperty);
            Assert.AreEqual(isNavProp, c.IsNavigationProperty, message);
            return c;
        }

        public static IColumnMapping MaxLength(this IColumnMapping c, int maxLength)
        {
            string message = string.Format("Property {0} max length should be {1}, but was {2}", c.PropertyName, maxLength, c.MaxLength);
            Assert.AreEqual(maxLength, c.MaxLength, message);
            return c;
        }

        public static IColumnMapping NavigationProperty(this IColumnMapping c, string navigationProperty)
        {
            string message = string.Format("Property {0} navigation property should be '{1}', but was '{2}'", c.PropertyName, navigationProperty, c.NavigationProperty);
            Assert.AreEqual(navigationProperty, c.NavigationProperty, message);
            return c;
        }
    }

    [TestFixture]
    public class MappingTest : TestBase
    {
        private const int nvarcharMax = 1073741823;

        [Test]
        public void TableNames()
        {
            using (var ctx = GetContext())
            {
                InitializeContext();

                var sw = new Stopwatch();
                sw.Restart();
                var dbmapping = ctx.Db();
                sw.Start();

                Console.WriteLine("Mapping took: {0}ms", sw.Elapsed.TotalMilliseconds);

                foreach (var tableMapping in dbmapping)
                {
                    Console.WriteLine("{0}: {1}.{2}", tableMapping.Type.FullName, tableMapping.Schema, tableMapping.TableName);
                }

                Assert.AreEqual(ctx.Db<Page>().TableName, "Pages");
                Assert.AreEqual(ctx.Db<PageTranslations>().TableName, "PageTranslations");

                Assert.AreEqual(ctx.Db<TestUser>().TableName, "Users");

                Assert.AreEqual(ctx.Db<MeteringPoint>().TableName, "MeteringPoints");

                Assert.AreEqual(ctx.Db<EmployeeTPH>().TableName, "Employees");
                Assert.AreEqual(ctx.Db<AWorkerTPH>().TableName, "Employees");
                Assert.AreEqual(ctx.Db<ManagerTPH>().TableName, "Employees");

                Assert.AreEqual(ctx.Db<ContractBase>().TableName, "Contracts");
                Assert.AreEqual(ctx.Db<Contract>().TableName, "Contracts");
                Assert.AreEqual(ctx.Db<ContractFixed>().TableName, "Contracts");
                Assert.AreEqual(ctx.Db<ContractStock>().TableName, "Contracts");
                Assert.AreEqual(ctx.Db<ContractKomb1>().TableName, "Contracts");
                Assert.AreEqual(ctx.Db<ContractKomb2>().TableName, "Contracts");

                Assert.AreEqual(ctx.Db<WorkerTPT>().TableName, "WorkerTPTs");
                Assert.AreEqual(ctx.Db<ManagerTPT>().TableName, "ManagerTPTs");
            }
        }

        [Test]
        public void ColumnNames_ComplexType()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<TestUser>();

                tableMapping.Col(x => x.Id)
                    .HasColumnName("Id")
                    .IsPk()
                    .IsFk(false)
                    .IsNavigationProperty(false);

                tableMapping.Col(x => x.FirstName)
                    .HasColumnName("Name")
                    .IsPk(false)
                    .IsFk(false)
                    .IsNavigationProperty(false)
                    .MaxLength(nvarcharMax);

                tableMapping.Col(x => x.LastName)
                    .HasColumnName("LastName")
                    .IsPk(false)
                    .IsFk(false)
                    .IsNavigationProperty(false)
                    .MaxLength(nvarcharMax);

                tableMapping.Col(x => x.Contact.PhoneNumber)
                    .HasColumnName("Contact_PhoneNumber")
                    .IsPk(false)
                    .IsFk(false)
                    .IsNavigationProperty(false)
                    .MaxLength(nvarcharMax);

                tableMapping.Col(x => x.Contact.Address.Country)
                    .HasColumnName("Contact_Address_Country")
                    .IsPk(false)
                    .IsFk(false)
                    .IsNavigationProperty(false)
                    .MaxLength(nvarcharMax);

                tableMapping.Col(x => x.Contact.Address.County)
                    .HasColumnName("Contact_Address_County")
                    .IsPk(false)
                    .IsFk(false)
                    .IsNavigationProperty(false)
                    .MaxLength(nvarcharMax);

                tableMapping.Col(x => x.Contact.Address.City)
                    .HasColumnName("Contact_Address_City")
                    .IsPk(false)
                    .IsFk(false)
                    .IsNavigationProperty(false)
                    .MaxLength(nvarcharMax);

                tableMapping.Col(x => x.Contact.Address.PostalCode)
                    .HasColumnName("Contact_Address_PostalCode")
                    .IsPk(false)
                    .IsFk(false)
                    .IsNavigationProperty(false)
                    .MaxLength(nvarcharMax);

                tableMapping.Col(x => x.Contact.Address.StreetAddress)
                    .HasColumnName("Contact_Address_StreetAddress")
                    .IsPk(false)
                    .IsFk(false)
                    .IsNavigationProperty(false)
                    .MaxLength(nvarcharMax);
            }
        }

        [Test]
        public void ColumnNames_TPT()
        {
            using (var ctx = GetContext())
            {
                var workerTptMapping = ctx.Db<WorkerTPT>();
                var columns = workerTptMapping.Columns;

                workerTptMapping.Col(x => x.Id)
                    .IsPk()
                    .IsFk(false)
                    .HasColumnName("Id")
                    .IsNavigationProperty(false);

                workerTptMapping.Col(x => x.Name)
                    .IsPk(false)
                    .IsFk(false)
                    .HasColumnName("Name")
                    .IsNavigationProperty(false)
                    .MaxLength(nvarcharMax);

                workerTptMapping.Col(x => x.JobTitle)
                    .IsPk(false)
                    .IsFk(false)
                    .HasColumnName("JobTitle")
                    .IsNavigationProperty(false)
                    .MaxLength(nvarcharMax);

                workerTptMapping.Col(x => x.Boss)
                    .IsPk(false)
                    .IsFk(false)
                    .HasColumnName("Boss_Id")
                    .IsNavigationProperty();

                workerTptMapping.Col(x => x.RefereeId)
                    .IsPk(false)
                    .IsFk()
                    .HasColumnName("RefereeId")
                    .IsNavigationProperty(false)
                    .NavigationProperty("Referee");

                workerTptMapping.Col(x => x.Referee)
                    .IsPk(false)
                    .IsFk()
                    .HasColumnName("RefereeId")
                    .IsNavigationProperty();

                /*
                var managetTptMapping = EfMap.Get<ManagerTPT>(ctx);
                columns = managetTptMapping.Columns;

                Cols(columns)
                    .Count(4)
                    .Col("Id")
                        .IsPk().Prop("Id").IsNavProp(false)
                        .And
                    .Col("Name")
                        .IsPk(false).Prop("Name").IsNavProp(false)
                        .And
                    .Col("JobTitle")
                        .IsPk(false).Prop("JobTitle").IsNavProp(false)
                        .And
                    .Col("Rank")
                        .IsPk(false).Prop("Rank").IsNavProp(false);*/
            }
        }

        [Test]
        public void ColumnNames_TPH_BaseClass()
        {
            using (var ctx = GetContext())
            {
                Console.WriteLine("EmployeeTPH");
                var tableMapping = ctx.Db<EmployeeTPH>();
                //var columns = tableMapping.Columns;
                /*
                Cols(tableMapping.Columns)
                    .Count(4)
                    .Col("Id")
                        .Prop("Id")
                        .IsPk()
                        .IsFk(false)
                        .NavProp(null)
                        .IsNavProp(false)
                        .And
                    .Col("Name")
                        .Prop("Name")
                        .IsFk(false)
                        .IsPk(false)
                        .IsNavProp(false)
                        .NavProp(null)
                        .And
                    .Col("JobTitle")
                        .Prop("Title")
                        .IsFk(false)
                        .IsPk(false)
                        .MaxLength(0)
                        .NavProp(null)
                        .IsNavProp(false)
                        .And
                    .Col("__employeeType")
                        .Prop("__employeeType"); 
                */
                /*
                Assert.AreEqual(4, columns.Length);

                AssertColumnName(columns, "Id", "Id");
                AssertColumnName(columns, "Name", "Name");
                AssertColumnName(columns, "JobTitle", "Title");
                AssertColumnName(columns, "__employeeType", "__employeeType");
                */
            }
        }

        [Test]
        public void ColumnNames_TPH_DerivedType_First()
        {
            using (var ctx = GetContext())
            {
                Console.WriteLine("WorkerTPH");
                var tableMapping = ctx.Db<AWorkerTPH>();
                var columns = tableMapping.Columns;
                Assert.AreEqual(6, columns.Length);

                /*
                Cols(columns)
                    .Count(6)
                    .Col("Id")
                        .IsPk().Prop("Id").IsNavProp(false)
                        .And
                    .Col("Name")
                        .IsPk(false).Prop("Name").IsNavProp(false)
                        .And
                    .Col("JobTitle")
                        .IsPk(false).Prop("Title").IsNavProp(false)
                        .And
                    .Col("BossId")
                        .IsPk(false).Prop("BossId").IsFk().NavProp("Boss")
                        .And
                    .Col("RefId")
                        .IsPk(false).Prop("RefId").IsNavProp(false)
                        .And
                    .Col("__employeeType")
                        .IsPk(false).Prop("__employeeType").IsNavProp(false);
                 */
            }
        }

        [Test]
        public void ColumnNames_TPH_DerivedType_NotFirst()
        {
            using (var ctx = GetContext())
            {
                Console.WriteLine("ManagerTPH");
                var tableMapping = ctx.Db<ManagerTPH>();
                var columns = tableMapping.Columns;
                Assert.AreEqual(6, columns.Length);

                /*
                AssertColumnName(columns, "Id", "Id");
                AssertColumnName(columns, "Name", "Name");
                AssertColumnName(columns, "JobTitle", "Title");
                AssertColumnName(columns, "Rank", "Rank");
                AssertColumnName(columns, "RefId1", "RefId");
                AssertColumnName(columns, "__employeeType", "__employeeType");
                 */
            }
        }

        [Test]
        public void ColumnNames_Simple()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<Page>();

                var columns = tableMapping.Columns;
                /*
                AssertColumnName(columns, "PageId", "PageId");
                AssertColumnName(columns, "Title", "Title");
                AssertColumnName(columns, "Content", "Content");
                AssertColumnName(columns, "ParentId", "ParentId");
                AssertColumnName(columns, "CreatedAt", "CreatedAt");
                AssertColumnName(columns, "ModifiedAt", "ModifiedAt");*/
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractBase()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<ContractBase>();

                var columns = tableMapping.Columns;
                Assert.AreEqual(18, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_Contract()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<Contract>();

                var columns = tableMapping.Columns;
                Assert.AreEqual(18, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractFixed()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<ContractFixed>();

                var columns = tableMapping.Columns;
                Assert.AreEqual(20, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractStock()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<ContractStock>();

                var columns = tableMapping.Columns;
                Assert.AreEqual(20, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractKomb1()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<ContractKomb1>();

                var columns = tableMapping.Columns;
                Assert.AreEqual(23, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractKomb2()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<ContractKomb2>();

                var columns = tableMapping.Columns;
                Assert.AreEqual(25, columns.Length);
            }
        }
    }
}
