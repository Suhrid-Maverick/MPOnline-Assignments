# Equals, ToString, and GetHashCode Methods — Comprehensive Notes

Every type in C# ultimately derives from `System.Object`, which defines three virtual methods that are fundamental to an object’s behaviour: `Equals`, `ToString`, and `GetHashCode`. These methods govern how objects are represented as strings, how they are compared for equality, and how they are hashed for use in dictionary keys and sets. Overriding them correctly is not just a matter of convenience—it is essential for the proper functioning of collections, debugging, logging, serialisation, and many other common operations.

The default implementations provided by `System.Object` are often insufficient for real‑world classes. `ToString()` simply returns the fully qualified type name, `Equals()` performs reference equality (for reference types), and `GetHashCode()` returns a hash based on the object’s memory address. While these defaults are acceptable for simple cases, they fail when we need to treat two distinct objects as logically equivalent (value equality) or when we want a human‑friendly string representation. This note explores each method in depth, explains the contracts they must honour, and offers practical guidelines for overriding them correctly, including modern C# alternatives like `IEquatable<T>` and the record syntax.

---

## 1. `object.ToString()` – Creating Meaningful String Representations

**Purpose:**  
`ToString()` is intended to return a human‑readable, culture‑insensitive string that describes the current instance. The default implementation in `System.Object` returns the assembly‑qualified type name (e.g., `"MyApp.Customer"`). This output is rarely useful except for diagnostic purposes that reveal the type identity.

**When to Override:**  
You should override `ToString()` for almost every class that represents a domain entity, a data transfer object (DTO), a value object, or any type that will be logged, displayed in a user interface, or printed in debug output. It provides a concise summary of the object’s state, making it easier to understand what the object represents without drilling into each property.

**Example of a Good Override:**

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }

    public override string ToString() => 
        $"{Name} (ID: {Id}, Price: {Price:C}, Created: {CreatedAt:yyyy-MM-dd})";
}
```

**Best Practices:**
- Include the most identifying and descriptive fields—typically the primary key, name, and a few key attributes.
- Avoid including very large collections or binary data; truncate if necessary.
- Use invariant culture for numeric and date formatting to ensure consistency across environments, unless the output is intended for UI with a specific culture.
- Do not throw exceptions inside `ToString()`—the method is used heavily in debuggers and logging, and unexpected exceptions can obscure real issues.
- For types that are primarily used as DTOs or view models, consider overriding `ToString()` to support `string` interpolation seamlessly.

**When Not to Override:**  
If your class is a low‑level framework type (e.g., a stream, a socket) where a string representation is not meaningful, or if the type is a simple wrapper that should defer to its wrapped value, you may keep the default. However, in most application‑level code, a meaningful override is highly recommended.

---

## 2. `object.Equals(object obj)` – Defining Value Equality

**Purpose:**  
`Equals` determines whether the current instance is equal to another object. The default implementation (for reference types) checks for *reference equality*—i.e., whether the two references point to the exact same memory location. For value types, the default is *value equality* (comparing all fields bit‑by‑bit), but this is rarely efficient or sufficient for custom structs.

**When to Override:**  
Override `Equals` when you want two distinct instances to be considered equal based on the values of their fields (value equality). This is typical for domain entities that have a unique identifier, for value objects (like money or date ranges), and for any class that will be stored in hash‑based collections as keys or membership tests.

**Equality Contract – The Four Properties:**  
Any implementation of `Equals` must adhere to these fundamental rules (as specified in the .NET documentation):

| Rule | Description |
|------|-------------|
| **Reflexive** | `x.Equals(x)` must return `true`. |
| **Symmetric** | If `x.Equals(y)` is `true`, then `y.Equals(x)` must also be `true`. |
| **Transitive** | If `x.Equals(y)` is `true` and `y.Equals(z)` is `true`, then `x.Equals(z)` must also be `true`. |
| **Consistent** | Multiple invocations of `x.Equals(y)` must consistently return the same result, provided the objects’ relevant state does not change. |
| **Non‑null**  | `x.Equals(null)` must always return `false`. |

Violating any of these rules leads to unpredictable behaviour in collections and algorithms that rely on equality, such as `List.Contains`, `Dictionary` lookups, and `HashSet` membership.

**Implementation Steps for a Typical Class:**

1. **Check for null** – if the other object is `null`, return `false` (unless this is also `null`, but that’s impossible for instance methods).
2. **Check reference equality** – if the references are the same, return `true` (optimisation).
3. **Check type compatibility** – ensure the other object is of the same type (or a compatible derived type). Often you use `as` or `is` pattern.
4. **Compare all equality‑relevant fields** – use `==` or `Equals` for each field. For string fields, consider case sensitivity (usually ordinal comparison).
5. **Avoid side‑effects** – `Equals` should not modify state or perform I/O.

**Example Implementation:**

```csharp
public class Employee
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }

    public override bool Equals(object obj)
    {
        // Step 1: null check
        if (obj is null) return false;

        // Step 2: reference equality
        if (ReferenceEquals(this, obj)) return true;

        // Step 3: type check
        if (obj.GetType() != this.GetType()) return false;

        // Step 4: cast and compare fields
        var other = (Employee)obj;
        return this.Id == other.Id 
            && string.Equals(this.Email, other.Email, StringComparison.OrdinalIgnoreCase)
            && this.FullName == other.FullName;
    }
}
```

**Important Nuances:**
- If your class is derived from another, overriding `Equals` must consider the base class’s equality logic (usually call `base.Equals(obj)`).
- For mutable objects, changing equality‑relevant fields after the object has been used as a key in a dictionary breaks the hash contract. It is advisable to design such classes as immutable, or at least document that mutating them invalidates their presence in hash structures.
- For structs, the default `Equals` uses reflection to compare fields; you should override it for performance and semantic control.

**Alternatives – `IEquatable<T>`:**  
To avoid boxing and type‑unsafe casts, implement the generic `IEquatable<T>` interface. This provides a strongly‑typed `Equals(T other)` method that is used by generic collections. The `object.Equals` override can then delegate to the generic version.

```csharp
public class Employee : IEquatable<Employee>
{
    // ... fields ...

    public bool Equals(Employee other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && Email == other.Email && FullName == other.FullName;
    }

    public override bool Equals(object obj) => Equals(obj as Employee);
}
```

Implementing `IEquatable<T>` is considered a best practice for value‑like types.

---

## 3. `object.GetHashCode()` – Providing a Hash for Collections

**Purpose:**  
`GetHashCode()` returns a 32‑bit signed integer that acts as a compact digest of the object’s state. This hash is used by hash‑based collections (`Dictionary<TKey, TValue>`, `HashSet<T>`, `ConcurrentDictionary`, etc.) to quickly organise objects into buckets for efficient lookup and membership testing.

**The Golden Rule:**  
If two objects are equal according to `Equals`, they **must** produce the same hash code. The converse is not required: two different objects may yield the same hash (a collision), but collisions degrade performance.

**When to Override:**  
You must override `GetHashCode` **whenever** you override `Equals`. If you do not, the default implementation (based on object identity) will violate the rule, causing a dictionary to fail to find a key that is logically equal but has a different reference.

**Implementation Guidelines:**
- Use the **same fields** that participate in `Equals`. If you add or change a field that is not used in equality, do not include it in the hash code.
- The algorithm should be **fast** – avoid expensive operations.
- The hash should be **well distributed** – it should not cluster, to minimise collisions.
- The hash should be **stable** over the object’s lifetime if the object is used as a key. If you mutate an object that is already in a hash set, its hash changes, and it will become “lost” in the collection. Therefore, either make the object immutable, or avoid using mutable objects as keys.

**Simple and Effective Approach using `HashCode.Combine` (available from .NET Core 2.1 / .NET Standard 2.1):**

```csharp
public override int GetHashCode() => HashCode.Combine(Id, Email, FullName);
```

`HashCode.Combine` takes up to eight values and combines them in a deterministic, well‑distributed manner, handling nulls gracefully.

**Alternative Manual Implementation (for older frameworks):**  
Use a prime multiplier and combine fields with XOR or addition:

```csharp
public override int GetHashCode()
{
    unchecked // Overflow is fine, just wrap
    {
        int hash = 17;
        hash = hash * 23 + Id.GetHashCode();
        hash = hash * 23 + (Email?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0);
        hash = hash * 23 + (FullName?.GetHashCode() ?? 0);
        return hash;
    }
}
```

**Important Caveats:**
- Do **not** use a constant value (e.g., `return 0`), as that will cause all objects to collide, turning hash tables into O(n) linear searches.
- Do **not** use only one field if that field has limited variation (e.g., a boolean).
- For collections as fields, use `Aggregate` or `HashCode.Combine` on the collection’s items (but be aware of performance).
- For string fields, consider the same case‑sensitivity rules as in `Equals`; otherwise, equality and hash codes may diverge.

**Performance Impact:**  
A poor hash function (many collisions) forces the collection to fall back to `Equals` checks for every item in a bucket, turning O(1) operations into O(n) in the worst case. Conversely, a good hash function yields near‑constant time lookups.

---

## 4. Modern C# Alternatives – `IEquatable<T>` and Records

### Implementing `IEquatable<T>`

As mentioned, implementing the generic interface provides type safety and avoids boxing. It is the recommended pattern for any type that overrides `Equals`. Most generic collections check for `IEquatable<T>` and call the generic method if available, bypassing the virtual `object.Equals` call for better performance.

**Example with Full Implementation:**

```csharp
public class Customer : IEquatable<Customer>
{
    public int Id { get; }
    public string Name { get; }

    public Customer(int id, string name) => (Id, Name) = (id, name);

    public bool Equals(Customer other) =>
        other is not null && Id == other.Id && Name == other.Name;

    public override bool Equals(object obj) => Equals(obj as Customer);

    public override int GetHashCode() => HashCode.Combine(Id, Name);
}
```

### Records (C# 9 and later)

The C# record type is a reference type that provides built‑in value semantics. When you declare a record with positional parameters (or with property definitions), the compiler automatically generates:

- An implementation of `Equals` (and `IEquatable<T>`) that compares all properties in the record.
- An implementation of `GetHashCode` that uses all properties.
- An implementation of `ToString` that outputs a formatted string showing all properties (similar to `{ TypeName { Prop1 = value, Prop2 = value } }`).
- Additional members like `Clone` and deconstructors.

**Example:**

```csharp
public record Customer(int Id, string Name, string Email);
```

This single line gives you:

- `Equals` that compares `Id`, `Name`, and `Email`.
- `GetHashCode` that combines these fields.
- `ToString` that returns something like `"Customer { Id = 1, Name = Alice, Email = alice@example.com }"`.

Records are ideal for DTOs, value objects, and any scenario where you want value equality without writing boilerplate. For mutable records, you can still override the default behaviour if needed, but records are best used as immutable.

**When to Use Each Approach:**

| Scenario | Recommendation |
|----------|----------------|
| Simple immutable data container | Use a `record` – minimal code, correct equality and hashing. |
| Mutable class with value semantics | Manually override `Equals`/`GetHashCode` and implement `IEquatable<T>`. |
| Class that should have reference equality (e.g., services, repositories) | Do not override; rely on default reference equality. |
| Value type (struct) | Always override `Equals` and `GetHashCode` for performance; consider implementing `IEquatable<T>`. |

---

## 5. Summary Table – Override Guidelines

| Method          | Default behaviour                            | Override when…                                                                                                 | Must also override … |
|-----------------|----------------------------------------------|----------------------------------------------------------------------------------------------------------------|----------------------|
| `ToString()`    | Returns fully qualified type name           | You want a readable, informative string for logging, debugging, or UI.                                        | None                 |
| `Equals(object)`| Reference equality (reference types); bitwise comparison (value types) | You need **value equality** – two distinct instances should be considered equal if their fields match.         | `GetHashCode()`      |
| `GetHashCode()` | Based on object identity (or field-based for structs) | You have overridden `Equals`. Also override if you plan to use the type as a key in hash collections.          | `Equals` (if not already) |

**Essential Rule of Thumb:**  
> Never override `Equals` without also overriding `GetHashCode`.  
> Never override `GetHashCode` without ensuring that the hash is consistent with `Equals`.  
> Treat equality‑relevant fields as immutable when the object is used in hash‑based containers.

---

## 6. Common Pitfalls and How to Avoid Them

1. **Inconsistent Hash when Mutable Keys:**  
   If you mutate an object after adding it to a `Dictionary` or `HashSet`, its hash code changes, and the collection will no longer be able to locate it. Always use immutable keys, or at least avoid mutation after insertion.

2. **Forgetting to Check Type in Equals:**  
   If you compare an object of a different type without checking, you may cause exceptions or incorrect results. Always use `obj is Type other` or `as` with a null check.

3. **Not Overriding GetHashCode When Using LINQ:**  
   `Distinct()`, `Union()`, `Intersect()`, and other set operations rely on the default equality comparer, which uses `GetHashCode` and `Equals`. Failure to override may lead to unexpected duplicates.

4. **Using Unreliable Hash Algorithms:**  
   Avoid using `XOR` alone for combining multiple fields – it is symmetric and can produce many collisions. Prefer `HashCode.Combine` or the prime‑based approach.

5. **Ignoring Culture in Strings:**  
   For case‑insensitive equality, use `StringComparer.OrdinalIgnoreCase` consistently in both `Equals` and `GetHashCode`. Using different comparers breaks the contract.

6. **Overriding ToString with Sensitive Data:**  
   Be mindful not to expose passwords, security tokens, or personally identifiable information (PII) in `ToString` output, as it may appear in logs or error messages.

---

By mastering these three methods and their relationships, you will write more robust and maintainable code. The ability to correctly define equality and string representation is a hallmark of a professional C# developer. With the advent of records, many of these tasks are automated, but it remains crucial to understand the underlying mechanics so that you can make informed decisions and troubleshoot issues when they arise.
