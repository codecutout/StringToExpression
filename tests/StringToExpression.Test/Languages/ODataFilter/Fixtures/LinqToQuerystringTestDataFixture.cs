using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Test.Fixtures
{
    public class LinqToQuerystringTestDataFixture
    {
        public readonly IQueryable<ConcreteClass> ConcreteCollection;
        
        public readonly IQueryable<ComplexClass> ComplexCollection;
        
        public readonly IQueryable<ConcreteClass> EdgeCaseCollection;
        
        public readonly IQueryable<NullableClass> NullableCollection;

        public readonly IQueryable<ConcreteClass> FunctionConcreteCollection;

        public const string guid0 = "1C1D07FC-0446-4C7B-8DFA-643D42EED070" ;
        public const string guid1 = "270FBCFF-8081-4ADC-B15F-FF9C979BB8AF";
        public const string guid2 = "2F26B3D4-B730-4958-8036-1F7FB9DE20A9";
        public const string guid3 = "2195EAF4-3692-4169-996E-A9B6F1560542";
        public const string guid4 = "3386D6C0-3F3A-4B77-8A6B-4360F75958CB";

        public LinqToQuerystringTestDataFixture()
        {
            var guidArray = new[]
            {
                new Guid(guid0),
                new Guid(guid1),
                new Guid(guid2),
                new Guid(guid3),
                new Guid(guid4),
            };

            ConcreteCollection = new List<ConcreteClass>
            {
                InstanceBuilders.BuildConcrete("Apple", 1, new DateTime(2002, 01, 01), true, 10000000000, 111.111, 111.111f, 0x00, 0.1m, guidArray[0]),
                InstanceBuilders.BuildConcrete("Apple", 2, new DateTime(2005, 01, 01), false, 30000000000, 333.333, 333.333f, 0x22, 0.3m, guidArray[2]),
                InstanceBuilders.BuildConcrete("Custard", 1, new DateTime(2003, 01, 01), true, 50000000000, 555.555, 555.555f, 0xDD, 0.5m, guidArray[4]),
                InstanceBuilders.BuildConcrete("Custard", 2, new DateTime(2002, 01, 01), false, 30000000000, 333.333, 333.333f, 0x00, 0.3m, guidArray[2]),
                InstanceBuilders.BuildConcrete("Custard", 3, new DateTime(2002, 01, 01), true, 40000000000, 444.444, 444.444f, 0x22, 0.4m, guidArray[3]),
                InstanceBuilders.BuildConcrete("Banana", 3, new DateTime(2003, 01, 01), false, 10000000000, 111.111, 111.111f, 0x00, 0.1m, guidArray[0]),
                InstanceBuilders.BuildConcrete("Eggs", 1, new DateTime(2005, 01, 01), true, 40000000000, 444.444, 444.444f, 0xCC, 0.4m, guidArray[3]),
                InstanceBuilders.BuildConcrete("Eggs", 3, new DateTime(2001, 01, 01), false, 20000000000, 222.222, 222.222f, 0xCC, 0.2m, guidArray[1]),
                InstanceBuilders.BuildConcrete("Dogfood", 4, new DateTime(2003, 01, 01), true, 30000000000, 333.333, 333.333f, 0xEE, 0.3m, guidArray[2]),
                InstanceBuilders.BuildConcrete("Dogfood", 4, new DateTime(2004, 01, 01), false, 10000000000, 111.111, 111.111f, 0xDD, 0.1m, guidArray[0]),
                InstanceBuilders.BuildConcrete("Dogfood", 5, new DateTime(2001, 01, 01), true, 20000000000, 222.222, 222.222f, 0xCC, 0.2m, guidArray[1])
            }.AsQueryable();

            FunctionConcreteCollection = new List<ConcreteClass>
            {
                InstanceBuilders.BuildConcrete("Saturday", 1, new DateTime(2001, 01, 01, 01, 05, 00), true),
                InstanceBuilders.BuildConcrete("Satnav", 2, new DateTime(2002, 01, 02, 06, 10, 10), false),
                InstanceBuilders.BuildConcrete("Saturnalia", 3, new DateTime(2003, 02, 02, 10, 10, 20), true),
                InstanceBuilders.BuildConcrete("Saturn", 4, new DateTime(2004, 04, 06, 10, 20, 30), true),
                InstanceBuilders.BuildConcrete("Monday", 5, new DateTime(2005, 06, 21, 13, 20, 40), true),
                InstanceBuilders.BuildConcrete("Tuesday", 5, new DateTime(2005, 06, 30, 20, 20, 50), true),
                InstanceBuilders.BuildConcrete("Burns", 5, new DateTime(2005, 10, 31, 23, 35, 50), true)
            }.AsQueryable();


            EdgeCaseCollection = new List<ConcreteClass>
            {
                InstanceBuilders.BuildConcrete("Apple\\Bob", 1, new DateTime(2002, 01, 01), true),
                InstanceBuilders.BuildConcrete("Apple\bBob", 1, new DateTime(2002, 01, 01), true),
                InstanceBuilders.BuildConcrete("Apple\tBob", 1, new DateTime(2002, 01, 01), true),
                InstanceBuilders.BuildConcrete("Apple\nBob", 1, new DateTime(2002, 01, 01), true),
                InstanceBuilders.BuildConcrete("Apple\fBob", 1, new DateTime(2002, 01, 01), true),
                InstanceBuilders.BuildConcrete("Apple\rBob", 1, new DateTime(2002, 01, 01), true),
                InstanceBuilders.BuildConcrete("Apple\"Bob", 1, new DateTime(2002  , 01, 01), true),
                InstanceBuilders.BuildConcrete("Apple'Bob", 1, new DateTime(2002, 01, 01), true),
                InstanceBuilders.BuildConcrete("x y & z", 1, new DateTime(2002, 01, 01), true),
            }.AsQueryable();

            ComplexCollection = new List<ComplexClass>
            {
                new ComplexClass { Title = "Charles", Concrete = InstanceBuilders.BuildConcrete("Apple", 5, new DateTime(2005, 01, 01), true) },
                new ComplexClass { Title = "Andrew", Concrete = InstanceBuilders.BuildConcrete("Custard", 3, new DateTime(2007, 01, 01), true) },
                new ComplexClass { Title = "David", Concrete = InstanceBuilders.BuildConcrete("Banana", 2, new DateTime(2003, 01, 01), false) },
                new ComplexClass { Title = "Edward", Concrete = InstanceBuilders.BuildConcrete("Eggs", 1, new DateTime(2000, 01, 01), true) },
                new ComplexClass { Title = "Boris", Concrete = InstanceBuilders.BuildConcrete("Dogfood", 4, new DateTime(2009, 01, 01), false) }
            }.AsQueryable();

            NullableCollection = new List<NullableClass>
            {
                InstanceBuilders.BuildNull(),
                InstanceBuilders.BuildNull(1, new DateTime(2002, 01, 01), true, 10000000000, 111.111, 111.111f, 0x00, guidArray[0], "Dogfood")
            }.AsQueryable();
        }

        public static class InstanceBuilders
        {
            public static ConcreteClass BuildConcrete(string name, int age, DateTime date, bool complete)
            {
                return new ConcreteClass { Name = name, Date = date, Age = age, Complete = complete };
            }

            public static EdgeCaseClass BuildEdgeCase(string name, int age, DateTime date, bool complete)
            {
                return new EdgeCaseClass { Name = name, Date = date, Age = age, Complete = complete };
            }

            public static ConcreteClass BuildConcrete(string name, int age, DateTime date, bool complete, long population, double value, float cost, byte code, decimal score, Guid guid)
            {
                return new ConcreteClass { Name = name, Date = date, Age = age, Complete = complete, Population = population, Value = value, Cost = cost, Code = code, Score = score, Guid = guid };
            }

            public static NullableClass BuildNull()
            {
                return new NullableClass();
            }

            public static NullableClass BuildNull(int? age, DateTime? date, bool? complete, long? population, double? value, float? cost, byte? code, Guid? guid, string name)
            {
                return new NullableClass { Date = date, Age = age, Complete = complete, Population = population, Value = value, Cost = cost, Code = code, Guid = guid, Name = name };
            }
        }

        public class ConcreteClass : IComparable<ConcreteClass>
        {
            public ConcreteClass()
            {
                Date = DateTime.UtcNow;
            }

            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime Date { get; set; }

            public bool Complete { get; set; }

            public int Age { get; set; }

            public List<EdgeCaseClass> Children { get; set; }

            public IEnumerable<string> StringCollection { get; set; }

            public long Population { get; set; }

            public double Value { get; set; }

            public float Cost { get; set; }

            public byte Code { get; set; }

            public Guid Guid { get; set; }

            public decimal Score { get; set; }

            public Color Color { get; set; }

            public int CompareTo(ConcreteClass other)
            {
                return String.CompareOrdinal(this.Name, other.Name);
            }
        }

        public enum Color
        {
            red,
            green,
            blue
        }

        public class EdgeCaseClass : IComparable<EdgeCaseClass>
        {
            public EdgeCaseClass()
            {
                Date = DateTime.UtcNow;
            }

            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime Date { get; set; }

            public bool Complete { get; set; }

            public int Age { get; set; }

            public int CompareTo(EdgeCaseClass other)
            {
                return String.CompareOrdinal(this.Name, other.Name);
            }
        }

        public class NullableContainer
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public List<NullableValue> Nullables { get; set; }
        }

        public class NullableValue
        {
            public int Id { get; set; }

            public int? Age { get; set; }
        }

        public class NullableClass
        {
            public int? Id { get; set; }

            public string Name { get; set; }

            public DateTime? Date { get; set; }

            public int? Age { get; set; }

            public bool? Complete { get; set; }

            public long? Population { get; set; }

            public double? Value { get; set; }

            public float? Cost { get; set; }

            public byte? Code { get; set; }

            public Guid? Guid { get; set; }

            public List<int?> NullableInts { get; set; }
        }

        public class ComplexClass
        {
            public int Id { get; set; }

            public ConcreteClass Concrete { get; set; }

            public List<string> StringCollection { get; set; }

            public List<ConcreteClass> ConcreteCollection { get; set; }

            public string Title { get; set; }

            public List<int> IntCollection { get; set; }
        }

        public class ComplexClassDto
        {
            public int Id { get; set; }

            public ConcreteClass Concrete { get; set; }

            public IEnumerable<string> StringCollection { get; set; }

            public List<ConcreteClass> ConcreteCollection { get; set; }

            public string Title { get; set; }

            public IEnumerable<int> IntCollection { get; set; }
        }

        public class NullableClassDto
        {
            public IEnumerable<int?> NullableCollection { get; set; }
        }

    }

}
