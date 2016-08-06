using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;
using Singular.Grid.Model;

namespace Singular.Grid.Tests
{
    [TestFixture]
    public class DefinitionsFluentApiTests
    {
        [SetUp]
        public void SetUp()
        {
            LoggingConfiguration config = new LoggingConfiguration();
            ConsoleTarget target = new ConsoleTarget();
            config.AddTarget("Console", target);
            config.AddRuleForAllLevels(target);

            LogManager.Configuration = config;
        }

        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void DefineAndBuildDataType()
        {
            ComplexDataType dataType1 = ComplexDataTypeBuilder.Define("tests.datatype1")
                .Extends("tests.anotherDataType")
                .Has("property1", p => p.FromDataType().References("tests.dt").Optional().Multiple())
                .Has("property2", p => p.ReadOnly().FromEnum().References("tests.enum"))
                .Has("property3", p => p.WriteOnly().FromPrimitiveType(PropertyTypes.Bool).Multiple())
                .Build();

            Assert.AreEqual("tests.datatype1", dataType1.Name.FullyQualifiedName);
            Assert.AreEqual("tests.anotherDataType", dataType1.Extends.FullyQualifiedName);
            Assert.AreEqual(3, dataType1.Properties.Count);

            PropertyDefinition p1 = dataType1.Properties["property1"];

            Assert.AreEqual(PropertyTypes.DataType, p1.Type);
            Assert.AreEqual("tests.dt", p1.TypeReferences.FullyQualifiedName);
            Assert.AreEqual(Presence.Optional, p1.Presence);
            Assert.AreEqual(Multiplicity.Multiple, p1.Multiplicity);
            Assert.AreEqual(PropertyModifier.ReadWrite, p1.Modifier);

            PropertyDefinition p2 = dataType1.Properties["property2"];

            Assert.AreEqual(PropertyTypes.Enum, p2.Type);
            Assert.AreEqual("tests.enum", p2.TypeReferences.FullyQualifiedName);
            Assert.AreEqual(Presence.Mandatory, p2.Presence);
            Assert.AreEqual(Multiplicity.Single, p2.Multiplicity);
            Assert.AreEqual(PropertyModifier.ReadOnly, p2.Modifier);

            PropertyDefinition p3 = dataType1.Properties["property3"];

            Assert.AreEqual(PropertyTypes.Bool, p3.Type);
            Assert.AreEqual(null, p3.TypeReferences);
            Assert.AreEqual(Presence.Mandatory, p3.Presence);
            Assert.AreEqual(Multiplicity.Multiple, p3.Multiplicity);
            Assert.AreEqual(PropertyModifier.WriteOnly, p3.Modifier);
        }

        [Test]
        public void DefineAndBuildDataTypeNotExtending()
        {
            ComplexDataType dataType1 = ComplexDataTypeBuilder.Define("tests.datatype1")
                .Has("property1", p => p.FromDataType().References("tests.baseDataType").Optional().Multiple())
                .Build();

            Assert.Null(dataType1.Extends);
        }

        [Test]
        public void DefineAspect()
        {
            ModelAspect modelAspect =
                ModelAspectBuilder.Define("this.is.an.aspect")
                    .Property("p1", p => p.FromPrimitiveType(PropertyTypes.Bool))
                    .Methods("m1", m =>
                        m.HasParameter("param1", p => p.FromPrimitiveType(PropertyTypes.Bool))
                            .Returns(r => r.FromPrimitiveType(PropertyTypes.Bool))
                    )
                    .Events("e1", e =>
                        e.HasParameter("param2", p => p.FromEnum().References("enum").Optional())
                    )
                    .Build();

            Assert.AreEqual("this.is.an.aspect", modelAspect.Name.FullyQualifiedName);
            Assert.AreEqual(1, modelAspect.Methods.Count);
            Assert.True(modelAspect.Methods.ContainsKey("m1"));
            Assert.AreEqual(1, modelAspect.Properties.Count);
            Assert.True(modelAspect.Properties.ContainsKey("p1"));
            Assert.AreEqual(1, modelAspect.Events.Count);
            Assert.True(modelAspect.Events.ContainsKey("e1"));
        }


        [Test]
        public void DefineAspectWithDuplicateMembers()
        {
            Assert.Throws<InvalidModelException>(() =>
                ModelAspectBuilder.Define("this.is.an.aspect")
                    .Property("name", p => p.FromPrimitiveType(PropertyTypes.Bool))
                    .Methods("name", m =>
                        m.HasParameter("param1", p => p.FromPrimitiveType(PropertyTypes.Bool))
                            .Returns(r => r.FromPrimitiveType(PropertyTypes.Bool))
                    )
                    .Build());

            Assert.Throws<InvalidModelException>(() =>
                ModelAspectBuilder.Define("this.is.an.aspect")
                    .Methods("name", m =>
                        m.HasParameter("param1", p => p.FromPrimitiveType(PropertyTypes.Bool))
                            .Returns(r => r.FromPrimitiveType(PropertyTypes.Bool)))
                    .Events("name", m =>
                        m.HasParameter("param2", p => p.FromPrimitiveType(PropertyTypes.Bool)))
                    .Build());

            Assert.Throws<InvalidModelException>(() =>
                ModelAspectBuilder.Define("this.is.an.aspect")
                    .Property("name", p => p.FromPrimitiveType(PropertyTypes.Bool))
                    .Events("name", m =>
                        m.HasParameter("param1", p => p.FromPrimitiveType(PropertyTypes.Bool)))
                    .Build());

            Assert.Throws<InvalidModelException>(() =>
                ModelAspectBuilder.Define("this.is.an.aspect")
                    .Events("name", m =>
                        m.HasParameter("param1", p => p.FromPrimitiveType(PropertyTypes.Bool)))
                    .Events("name", m =>
                        m.HasParameter("param2", p => p.FromPrimitiveType(PropertyTypes.Bool)))
                    .Build());

            Assert.Throws<InvalidModelException>(() =>
                ModelAspectBuilder.Define("this.is.an.aspect")
                    .Methods("name", m =>
                        m.HasParameter("param1", p => p.FromPrimitiveType(PropertyTypes.Bool))
                            .Returns(r => r.FromPrimitiveType(PropertyTypes.Bool)))
                    .Methods("name", m =>
                        m.HasParameter("param2", p => p.FromPrimitiveType(PropertyTypes.Bool)))
                    .Build());

            Assert.Throws<InvalidModelException>(() =>
                ModelAspectBuilder.Define("this.is.an.aspect")
                    .Property("name", p => p.FromPrimitiveType(PropertyTypes.Bool))
                    .Property("name", p => p.FromPrimitiveType(PropertyTypes.Bool))
                    .Build());

            Assert.Throws<InvalidModelException>(() => ModelAspectBuilder.Define("this.is.an.aspect").Build());
        }

        [Test]
        public void DefineDataTypeOfIncorrectType()
        {
            Assert.Throws<InvalidModelException>(() => ComplexDataTypeBuilder.Define("tests.datatype1")
                .Has("property3", p => p.FromPrimitiveType(PropertyTypes.Enum).Multiple())
                .Build());
        }

        [Test]
        public void DefineDataTypeWithInvalidProperties()
        {
            Assert.Throws<InvalidModelException>(() => ComplexDataTypeBuilder.Define("tests.datatype1").Build());
            Assert.Throws<InvalidModelException>(
                () =>
                    ComplexDataTypeBuilder.Define("tests.datatype1")
                        .Has("p", p => p.FromPrimitiveType(PropertyTypes.Bool))
                        .Has("p", p => p.FromPrimitiveType(PropertyTypes.Byte))
                        .Build());
        }

        [Test]
        public void DefineDataTypeWithInvalidPropertiesNames()
        {
            Assert.Throws<InvalidModelException>(() => ComplexDataTypeBuilder.Define("tests.datatype1")
                .Has(null, p => p.FromPrimitiveType(PropertyTypes.Enum).Multiple())
                .Build());

            Assert.Throws<InvalidModelException>(() => ComplexDataTypeBuilder.Define("tests.datatype1")
                .Has("", p => p.FromPrimitiveType(PropertyTypes.Enum).Multiple())
                .Build());

            Assert.Throws<InvalidModelException>(() => ComplexDataTypeBuilder.Define("tests.datatype1")
                .Has(" ", p => p.FromPrimitiveType(PropertyTypes.Enum).Multiple())
                .Build());

            Assert.Throws<InvalidModelException>(() => ComplexDataTypeBuilder.Define("tests.datatype1")
                .Has("1ddas", p => p.FromPrimitiveType(PropertyTypes.Enum).Multiple())
                .Build());

            Assert.Throws<InvalidModelException>(() => ComplexDataTypeBuilder.Define("tests.datatype1")
                .Has("asd.sadsd", p => p.FromPrimitiveType(PropertyTypes.Enum).Multiple())
                .Build());
        }

        [Test]
        public void DefineDataTypeWithValidPropertiesNames()
        {
            Assert.Throws<InvalidModelException>(() => ComplexDataTypeBuilder.Define("tests.datatype1")
                .Has("_asdasd", p => p.FromPrimitiveType(PropertyTypes.Enum).Multiple())
                .Build());

            Assert.Throws<InvalidModelException>(() => ComplexDataTypeBuilder.Define("tests.datatype1")
                .Has("asd_dsa_sd1", p => p.FromPrimitiveType(PropertyTypes.Enum).Multiple())
                .Build());

            Assert.Throws<InvalidModelException>(() => ComplexDataTypeBuilder.Define("tests.datatype1")
                .Has("a1", p => p.FromPrimitiveType(PropertyTypes.Enum).Multiple())
                .Build());

            Assert.Throws<InvalidModelException>(() => ComplexDataTypeBuilder.Define("tests.datatype1")
                .Has("_1", p => p.FromPrimitiveType(PropertyTypes.Enum).Multiple())
                .Build());

            Assert.Throws<InvalidModelException>(() => ComplexDataTypeBuilder.Define("tests.datatype1")
                .Has("__", p => p.FromPrimitiveType(PropertyTypes.Enum).Multiple())
                .Build());
        }

        [Test]
        public void DefineEnumeration()
        {
            Enumeration enum1 = EnumerationBuilder.Define("this.is.an.enumeration")
                .Extends("another.enumeration")
                .HasMember("Member1")
                .HasMember("Member2")
                .HasMember("Member3")
                .Build();

            Enumeration enum2 = EnumerationBuilder.Define("yet.another.enumeration")
                .HasMember("m1")
                .HasMember("m2")
                .Build();

            Assert.AreEqual("this.is.an.enumeration", enum1.Name.FullyQualifiedName);
            Assert.AreEqual("another.enumeration", enum1.Extends.FullyQualifiedName);
            Assert.AreEqual(3, enum1.Values.Count);
            Assert.AreEqual("Member1", enum1.Values[0]);
            Assert.AreEqual("Member2", enum1.Values[1]);
            Assert.AreEqual("Member3", enum1.Values[2]);

            Assert.AreEqual("yet.another.enumeration", enum2.Name.FullyQualifiedName);
            Assert.AreEqual(null, enum2.Extends);
            Assert.AreEqual(2, enum2.Values.Count);
            Assert.AreEqual("m1", enum2.Values[0]);
            Assert.AreEqual("m2", enum2.Values[1]);
        }

        [Test]
        public void DefineModel()
        {
            Model.Model model1 =
                ModelBuilder.Define("this.is.a.model")
                    .Extends("extended.model")
                    .Aspect("this.is.an.aspect")
                    .Aspect("another.aspect")
                    .Build();

            Assert.AreEqual("this.is.a.model", model1.Name.FullyQualifiedName);
            Assert.AreEqual("extended.model", model1.Extends.FullyQualifiedName);
            Assert.AreEqual(2, model1.Aspects.Count);
            Assert.AreEqual("this.is.an.aspect", model1.Aspects[0].FullyQualifiedName);
            Assert.AreEqual("another.aspect", model1.Aspects[1].FullyQualifiedName);

            Model.Model model2 =
                ModelBuilder.Define("this.is.a.model")
                    .Aspect("another.aspect")
                    .Build();

            Assert.IsNull(model2.Extends);
        }

        [Test]
        public void DefineModelWithoutAspects()
        {
            Assert.Throws<InvalidModelException>(
                () => ModelBuilder.Define("this.is.a.model").Extends("extended.model").Build());
        }

        [Test]
        public void InvalidEnumerationDefinitions()
        {
            Assert.Throws<InvalidModelException>(() => EnumerationBuilder.Define("name").Build());
            Assert.Throws<InvalidModelException>(() => EnumerationBuilder.Define("name").HasMember("").Build());
            Assert.Throws<InvalidModelException>(() => EnumerationBuilder.Define("name").HasMember("a.b").Build());
            Assert.Throws<InvalidModelException>(
                () => EnumerationBuilder.Define("name").HasMember("a").HasMember("a").Build());
        }

        [Test]
        public void ModelNameConstructor()
        {
            ModelName mn1 = new ModelName("test.model.name");

            Assert.AreEqual("name", mn1.Name);
            Assert.AreEqual("test.model", mn1.Namespace);
            Assert.AreEqual("test.model.name", mn1.FullyQualifiedName);

            ModelName mn2 = new ModelName("otherName");

            Assert.AreEqual("otherName", mn2.Name);
            Assert.AreEqual(string.Empty, mn2.Namespace);
            Assert.AreEqual("otherName", mn2.FullyQualifiedName);

            Assert.Throws<InvalidModelException>(() => new ModelName(""));
            Assert.Throws<InvalidModelException>(() => new ModelName("."));
            Assert.Throws<InvalidModelException>(() => new ModelName("1.a"));
            Assert.Throws<InvalidModelException>(() => new ModelName("a.1a"));
            Assert.Throws<InvalidModelException>(() => new ModelName("a..b"));
            Assert.Throws<InvalidModelException>(() => new ModelName("a-d"));
        }
    }
}