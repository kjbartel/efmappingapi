using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EntityFramework.MappingAPI.Test.CodeFirst.Domain;
using NUnit.Framework;

namespace EntityFramework.MappingAPI.Test.CodeFirst
{
    [TestFixture]
    public class MappingTest : TestBase
    {
        [Test]
        public void TableNames()
        {
            using (var ctx = GetContext())
            {
                InitializeContext();

                var sw = new Stopwatch();
                sw.Restart();
                var dbmapping = EfMap.Get(ctx);
                sw.Start();

                Console.WriteLine("Mapping took: {0}ms", sw.Elapsed.TotalMilliseconds);

                var tableMappings = dbmapping.Tables;

                foreach (var tableMapping in tableMappings)
                {
                    Console.WriteLine("{0}: {1}.{2}", tableMapping.TypeFullName, tableMapping.Schema, tableMapping.TableName);
                }

                AssertTableName<Page>(tableMappings, "Pages");
                AssertTableName<PageTranslations>(tableMappings, "PageTranslations");

                AssertTableName<TestUser>(tableMappings, "Users");

                AssertTableName<MeteringPoint>(tableMappings, "MeteringPoints");

                AssertTableName<EmployeeTPH>(tableMappings, "Employees");
                AssertTableName<AWorkerTPH>(tableMappings, "Employees");
                AssertTableName<ManagerTPH>(tableMappings, "Employees");

                AssertTableName<ContractBase>(tableMappings, "Contracts");
                AssertTableName<Contract>(tableMappings, "Contracts");
                AssertTableName<ContractFixed>(tableMappings, "Contracts");
                AssertTableName<ContractStock>(tableMappings, "Contracts");
                AssertTableName<ContractKomb1>(tableMappings, "Contracts");
                AssertTableName<ContractKomb2>(tableMappings, "Contracts");

                AssertTableName<WorkerTPT>(tableMappings, "WorkerTPTs");
                AssertTableName<ManagerTPT>(tableMappings, "ManagerTPTs");
            }
        }

        private void AssertTableName<T>(IEnumerable<ITableMapping> tableMappings, string tableName)
        {
            var typeName = typeof(T).FullName;
            Assert.True(tableMappings.Any(x => x.TableName == tableName && x.TypeFullName == typeName), "Type '" + typeName + "' should be mapped to table '" + tableName + "'");
        }

        private void AssertColumnName(IEnumerable<IColumnMapping> columnMappings, string colName, string propName)
        {
            Console.WriteLine("prop: {0} > col: {1}", propName, colName);
            var col = columnMappings.FirstOrDefault(x => x.ColumnName == colName && x.PropertyName == propName);
            Assert.IsNotNull(col);
        }

        private ColsAssert Cols(IEnumerable<IColumnMapping> columnMappings)
        {
            return new ColsAssert(columnMappings);
        }

        public class ColAssert
        {
            public ColsAssert And { get; private set; }

            private readonly IColumnMapping _columnMapping;
            private readonly string _cn;

            public ColAssert(ColsAssert colsAssert, IColumnMapping columnMapping)
            {
                And = colsAssert;
                _columnMapping = columnMapping;
                _cn = columnMapping.ColumnName;
            }

            public ColAssert IsPk(bool isPk = true)
            {
                Assert.AreEqual(isPk, _columnMapping.IsPk);
                return this;
            }

            public ColAssert Prop(string propName)
            {
                Assert.AreEqual(propName, _columnMapping.PropertyName);
                return this;
            }

            public ColAssert IsNavProp(bool isNavProp)
            {
                Assert.AreEqual(isNavProp, _columnMapping.IsNavigationProperty);
                return this;
            }

            public ColAssert IsFk(bool isFk = true)
            {
                if (isFk)
                {
                    Assert.IsTrue(_columnMapping.IsFk, _columnMapping.ColumnName + " should be a foreign key!");
                }
                else
                {
                    Assert.IsFalse(_columnMapping.IsFk, _columnMapping.ColumnName + " should NOT be a foreign key!");
                }
                return this;
            }

            public ColAssert NavProp(string propName)
            {
                Assert.AreEqual(propName, _columnMapping.NavigationProperty, _cn + " should have navigation property named '" + propName + "'");
                return this;
            }
        }

        public class ColsAssert
        {
            private readonly Dictionary<string, IColumnMapping> _columnMappings;
            public ColsAssert(IEnumerable<IColumnMapping> columnMappings)
            {
                _columnMappings = columnMappings.ToDictionary(x => x.ColumnName);
            }

            public ColAssert Col(string colName)
            {
                Assert.IsTrue(_columnMappings.ContainsKey(colName), "Column mappings does not contain column named '" + colName + "'");
                return new ColAssert(this, _columnMappings[colName]);
            }

            public ColsAssert Count(int count)
            {
                Assert.AreEqual(count, _columnMappings.Count);
                return this;
            }
        }

        [Test]
        public void ColumnNames_ComplexType()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<TestUser>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(9, columns.Length);

                AssertColumnName(columns, "Id", "Id");
                AssertColumnName(columns, "Name", "FirstName");
                AssertColumnName(columns, "LastName", "LastName");
                AssertColumnName(columns, "Contact_PhoneNumber", "Contact.PhoneNumber");
                AssertColumnName(columns, "Contact_Address_Country", "Contact.Address.Country");
                AssertColumnName(columns, "Contact_Address_County", "Contact.Address.County");
                AssertColumnName(columns, "Contact_Address_City", "Contact.Address.City");
                AssertColumnName(columns, "Contact_Address_PostalCode", "Contact.Address.PostalCode");
                AssertColumnName(columns, "Contact_Address_StreetAddress", "Contact.Address.StreetAddress");
            }
        }

        [Test]
        public void ColumnNames_TPT()
        {
            using (var ctx = GetContext())
            {
                var tableMapping = EfMap.Get<WorkerTPT>(ctx);
                var columns = tableMapping.Columns;

                Cols(columns)
                    .Count(5)
                    .Col("Id")
                        .IsPk().Prop("Id").IsNavProp(false)
                        .And
                    .Col("Name")
                        .IsPk(false).Prop("Name").IsNavProp(false)
                        .And
                    .Col("JobTitle")
                        .IsPk(false).Prop("JobTitle").IsNavProp(false)
                        .And
                    .Col("Boss_Id")
                        .IsPk(false).Prop("Boss").IsNavProp(true)
                        .And
                    .Col("RefereeId")
                        .IsPk(false).Prop("RefereeId").NavProp("Referee").IsFk();

                tableMapping = EfMap.Get<ManagerTPT>(ctx);
                columns = tableMapping.Columns;

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
                        .IsPk(false).Prop("Rank").IsNavProp(false);
            }
        }

        [Test]
        public void ColumnNames_TPH_BaseClass()
        {
            using (var ctx = GetContext())
            {
                Console.WriteLine("EmployeeTPH");
                var tableMapping = EfMap.Get<EmployeeTPH>(ctx);
                var columns = tableMapping.Columns;
                Assert.AreEqual(4, columns.Length);

                AssertColumnName(columns, "Id", "Id");
                AssertColumnName(columns, "Name", "Name");
                AssertColumnName(columns, "JobTitle", "Title");
                AssertColumnName(columns, "__employeeType", "__employeeType");
            }
        }

        [Test]
        public void ColumnNames_TPH_DerivedType_First()
        {
            using (var ctx = GetContext())
            {
                Console.WriteLine("WorkerTPH");
                var tableMapping = EfMap.Get<AWorkerTPH>(ctx);
                var columns = tableMapping.Columns;
                Assert.AreEqual(6, columns.Length);


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
            }
        }

        [Test]
        public void ColumnNames_TPH_DerivedType_NotFirst()
        {
            using (var ctx = GetContext())
            {
                Console.WriteLine("ManagerTPH");
                var tableMapping = EfMap.Get<ManagerTPH>(ctx);
                var columns = tableMapping.Columns;
                Assert.AreEqual(6, columns.Length);

                AssertColumnName(columns, "Id", "Id");
                AssertColumnName(columns, "Name", "Name");
                AssertColumnName(columns, "JobTitle", "Title");
                AssertColumnName(columns, "Rank", "Rank");
                AssertColumnName(columns, "RefId1", "RefId");
                AssertColumnName(columns, "__employeeType", "__employeeType");
            }
        }

        [Test]
        public void ColumnNames_Simple()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<Page>(ctx);

                var columns = tableMapping.Columns;

                AssertColumnName(columns, "PageId", "PageId");
                AssertColumnName(columns, "Title", "Title");
                AssertColumnName(columns, "Content", "Content");
                AssertColumnName(columns, "ParentId", "ParentId");
                AssertColumnName(columns, "CreatedAt", "CreatedAt");
                AssertColumnName(columns, "ModifiedAt", "ModifiedAt");
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractBase()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<ContractBase>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(18, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_Contract()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<Contract>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(18, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractFixed()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<ContractFixed>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(20, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractStock()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<ContractStock>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(20, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractKomb1()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<ContractKomb1>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(23, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractKomb2()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<ContractKomb2>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(25, columns.Length);
            }
        }
    }
}
