using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using MemoryPack;

namespace MemoryPackTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var v = new Person { Age = 40, Name = "John" };

            var bin = MemoryPackSerializer.Serialize(v);
            var val = MemoryPackSerializer.Deserialize<Person>(bin);

            var value = new TestStruct { A = 1, B = 2 };
            var bytes = SerializeBlittable(value);

            Console.WriteLine(BitConverter.ToString(bytes)); // 출력 결과: 01-00-00-00-02-00-00-00

            value = new TestStruct { A = 0, B = 0 };

            DeserializeBlittable(bytes, out value);

            Console.WriteLine(value); // 출력 결과: A: 1, B: 2

        }

        public static byte[] SerializeBlittable<T>(in T? value)
        {
            if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                var buffer = GC.AllocateUninitializedArray<byte>(Unsafe.SizeOf<T>());
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetArrayDataReference(buffer), value);
                return buffer;
            }

            return Array.Empty<byte>();
        }

        private static void DeserializeBlittable<T>(byte[] buffer, out T value)
        {
            if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                value = Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetArrayDataReference(buffer));
                return;
            }

            value = default!;
        }
    }

    struct TestStruct
    {
        public int A { get; set; }
        public int B { get; set; }
    }

    [MemoryPackable]
    public partial class Person
    {
        public string? Name { get; set; }
        public int Age { get; set; }
    }
}
