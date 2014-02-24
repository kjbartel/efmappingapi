using System;
using EntityFramework.MappingAPI.Extensions;
using EntityFramework.MappingAPI.Test.CodeFirst.Domain;
using NUnit.Framework;

namespace EntityFramework.MappingAPI.Test.CodeFirst
{
    public class TphTest : TestBase
    {
        [Test]
        public void Employee_BaseClass()
        {
            using (var ctx = GetContext())
            {
                var map = ctx.Db<EmployeeTPH>();
                Console.WriteLine("{0}:{1}", map.Type, map.TableName);

                map.Prop(x => x.Id)
                    .HasColumnName("Id")
                    .IsPk()
                    .IsFk(false);

                map.Prop(x => x.Name)
                    .HasColumnName("Name")
                    .MaxLength(NvarcharMax)
                    .NavigationProperty(null)
                    .IsPk(false)
                    .IsFk(false);

                Assert.IsNull(map.Prop(x => x.NameWithTitle));

                map.Prop(x => x.Title)
                    .HasColumnName("JobTitle")
                    .IsPk(false)
                    .IsFk(false)
                    .MaxLength(NvarcharMax)
                    .IsNavigationProperty(false);

                map.Prop("__employeeType")
                    .HasColumnName("__employeeType")
                    .IsPk(false)
                    .IsFk(false)
                    .IsDiscriminator();
            }
        }

        [Test]
        public void Employee_DerivedType_First()
        {
            using (var ctx = GetContext())
            {
                var map = ctx.Db<AWorkerTPH>();
                Console.WriteLine("{0}:{1}", map.Type, map.TableName);



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
        public void Employee_DerivedType_NotFirst()
        {
            using (var ctx = GetContext())
            {
                var map = ctx.Db<ManagerTPH>();
                Console.WriteLine("{0}:{1}", map.Type, map.TableName);

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
        public void Contract_ContractBase()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<ContractBase>();

                var columns = tableMapping.Properties;
                Assert.AreEqual(19, columns.Length);
            }
        }

        [Test]
        public void Contract_Contract()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<Contract>();

                var columns = tableMapping.Properties;
                Assert.AreEqual(19, columns.Length);
            }
        }

        [Test]
        public void Contract_ContractFixed()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<ContractFixed>();

                var columns = tableMapping.Properties;
                Assert.AreEqual(21, columns.Length);
            }
        }

        [Test]
        public void Contract_ContractStock()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<ContractStock>();

                var columns = tableMapping.Properties;
                Assert.AreEqual(21, columns.Length);
            }
        }

        [Test]
        public void Contract_ContractKomb1()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<ContractKomb1>();

                var columns = tableMapping.Properties;
                Assert.AreEqual(24, columns.Length);
            }
        }

        [Test]
        public void Contract_ContractKomb2()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<ContractKomb2>();

                var columns = tableMapping.Properties;
                Assert.AreEqual(26, columns.Length);
            }
        }
    }
}