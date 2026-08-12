# Collection Framework in C# — Well-Defined Notes

The C# **Collection Framework** (found in the `System.Collections` and `System.Collections.Generic` namespaces) provides a set of strongly-typed classes and interfaces for storing, retrieving, and manipulating groups of related objects. Collections replace raw arrays when you need dynamic sizing, ordering, key-based lookup, or specialised behaviour (queue/stack semantics). They form the foundation of data management in most .NET applications, enabling efficient and type‑safe operations.

---

## 1. Two Generations of Collections

The framework evolved from non‑generic legacy collections to the modern generic ones. **Always prefer generic collections** unless you are working with legacy code or need to support multiple types dynamically (rare).

### Non-generic (`System.Collections`) — legacy, stores `object`

| Class | Purpose |
|-------|---------|
| `ArrayList`     | Dynamically-sized array of `object` |
| `Hashtable`     | Key/value pairs, no type safety |
| `Queue`         | FIFO collection |
| `Stack`         | LIFO collection |
| `SortedList`    | Key/value pairs sorted by key |

**Drawbacks:**
- **Boxing/unboxing** – value types are boxed when added and unboxed when retrieved, causing performance overhead and memory churn.
- **No compile‑time type safety** – you can accidentally add a `string` to an `ArrayList` of `int`s, leading to runtime `InvalidCastException`.
- **No generic methods** – you must manually cast, which clutters code.

> Use these only when interfacing with older APIs or .NET Framework 1.x code.

---

### Generic (`System.Collections.Generic`) — modern, type-safe

| Class | Purpose |
|-------|---------|
| `List<T>`         | Dynamic array of type `T` |
| `Dictionary<TKey, TValue>` | Hash map of key→value |
| `HashSet<T>`      | Unique unordered elements |
| `SortedSet<T>`    | Unique elements in sorted order |
| `Queue<T>`        | FIFO |
| `Stack<T>`        | LIFO |
| `LinkedList<T>`   | Doubly linked list |
| `SortedList<TKey, TValue>` | Sorted key/value (array-backed) |
| `SortedDictionary<TKey, TValue>` | Sorted key/value (tree-backed) |
| `ConcurrentDictionary<TKey, TValue>` | Thread-safe dictionary (from `System.Collections.Concurrent`) |

**Advantages:**
- **Type safety** – the compiler enforces the element type.
- **No boxing** – value types are stored directly.
- **Rich generic APIs** – methods like `Find`, `Sort`, `ConvertAll` operate with type parameters.
- **Better performance** – both memory and CPU.

> **Note:** `SortedList` and `SortedDictionary` both store keys in sorted order. The former uses a list (array) internally, offering O(1) indexed lookup but O(n) insert/delete; the latter uses a balanced tree, offering O(log n) insert/delete but no indexed access. Choose based on your access patterns.

---

## 2. Core Interfaces

These interfaces define the contracts that collections implement. They enable polymorphic code, LINQ, and custom collection types.

| Interface | Meaning |
|-----------|---------|
| `IEnumerable<T>`      | Enables `foreach` iteration (forward‑only, read‑only cursor) |
| `ICollection<T>`      | Base for all generic collections (Count, Add, Remove, CopyTo, IsReadOnly) |
| `IList<T>`            | Indexed access (List<T>); supports positional inserts/removals |
| `IDictionary<TKey,TValue>` | Key/value access; adds indexer and key‑based operations |
| `ISet<T>`             | Set semantics (no duplicates); provides set operations (Union, Intersect, etc.) |
| `IComparer<T>`        | External comparison strategy for sorting or ordering |
| `IEqualityComparer<T>`| Defines equality for hashing (used by dictionaries and sets) |

**Key relationships:**
- `IList<T>` extends `ICollection<T>` extends `IEnumerable<T>`.
- `IDictionary<TKey,TValue>` extends `ICollection<KeyValuePair<TKey,TValue>>` and `IEnumerable`.
- Most collections also implement the non‑generic counterparts for backward compatibility.

**Practical use:**
- Accept `IEnumerable<T>` for read‑only enumeration.
- Accept `IList<T>` when you need index‑based modification.
- Use `IComparer<T>` to supply custom sort orders (e.g., case‑insensitive string comparison).
- Use `IEqualityComparer<T>` to control how keys are hashed and compared in dictionaries and sets.

---

## 3. Why Use Collections Instead of Arrays?

Arrays (`T[]`) are the most primitive data structure. Collections offer significant advantages:

1. **Dynamic size** — grow and shrink automatically (e.g., `List<T>.Add` resizes as needed).
2. **Type safety** (generic collections) — compile‑time type checking eliminates casting errors.
3. **Rich API** — built‑in methods for common operations:
   - `Add`, `AddRange`, `Insert`
   - `Remove`, `RemoveAt`, `RemoveAll`
   - `Contains`, `Find`, `FindAll`, `IndexOf`
   - `Sort`, `Reverse`, `ConvertAll`, `ForEach`
4. **Specialised structures** — queues, stacks, dictionaries, sets are optimised for specific use cases, providing better performance than custom implementations with arrays.
5. **LINQ support** — collections implement `IEnumerable<T>`, enabling fluent queries:
   ```csharp
   var adults = people.Where(p => p.Age >= 18).OrderBy(p => p.Name).ToList();
   ```
6. **Async and thread‑safe variants** — concurrent collections (`ConcurrentBag`, `ConcurrentDictionary`) are designed for multi‑threaded scenarios, which raw arrays cannot handle safely.

> **When to use arrays:** When the size is fixed and known at compile‑time, or when you need the absolute minimum overhead for performance‑critical code. For most other scenarios, collections are superior.

---

## 4. Quick Example

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

// List<T> – dynamic array
List<int> nums = new() { 5, 2, 9, 1 };
nums.Add(7);
nums.Sort();               // now {1,2,5,7,9}
Console.WriteLine(nums[2]); // 5

// Dictionary<TKey,TValue> – key/value lookup
Dictionary<string, int> ages = new()
{
    ["Alice"] = 22,
    ["Bob"]   = 19
};
if (ages.TryGetValue("Alice", out int age))
    Console.WriteLine($"Alice is {age}");

// HashSet<T> – unique elements (duplicates ignored)
HashSet<string> tags = new() { "csharp", "dotnet", "csharp" };
Console.WriteLine(tags.Count); // 2

// LINQ integration
var sortedEven = nums.Where(n => n % 2 == 0).OrderByDescending(n => n);
Console.WriteLine(string.Join(", ", sortedEven)); // "8, 2" (if we had 8, but we have only even: 2)
```

---

## 5. Choosing the Right Collection

The table below gives a quick decision guide based on common requirements. Consider both functional needs and performance characteristics.

| Need | Use |
|------|-----|
| Indexed access, frequent traversal, dynamic sizing | `List<T>` |
| Fast lookup by unique key (e.g., ID → object) | `Dictionary<TKey, TValue>` |
| Unique items (no duplicates) with fast membership test | `HashSet<T>` |
| FIFO order (first‑in, first‑out) processing | `Queue<T>` |
| LIFO order (last‑in, first‑out) processing | `Stack<T>` |
| Sorted unique items with efficient ordering | `SortedSet<T>` |
| Frequent insert/remove in the middle of the sequence | `LinkedList<T>` |
| Sorted key‑value pairs with fast indexed lookup by key | `SortedList<TKey, TValue>` (small, frequent reads) |
| Sorted key‑value pairs with fast insert/delete | `SortedDictionary<TKey, TValue>` (large, frequent writes) |
| Thread‑safe key/value access from multiple threads | `ConcurrentDictionary<TKey, TValue>` |

**Additional tips:**
- For read‑only operations, use `IReadOnlyList<T>` or `IReadOnlyCollection<T>` to signal immutability.
- If you need a collection that behaves like an array but with dynamic size, `List<T>` is almost always the right choice.
- For large collections with many lookups, `Dictionary<TKey, TValue>` offers O(1) average access time; just ensure your key type has a good `GetHashCode()` implementation.
- For custom sorting, pass an `IComparer<T>` to the `Sort` method of `List<T>` or to the constructor of `SortedSet<T>` / `SortedDictionary`.

---

The collection framework is the backbone of almost every C# application — mastering it is essential for writing clean, efficient, and maintainable code. By understanding the trade‑offs between different collection types and their interfaces, you can choose the optimal data structure for every scenario, leading to better performance and fewer bugs.
