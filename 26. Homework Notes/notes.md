# Homework Notes — SDLC / Entity Framework / ORM / HTML / CSS / ER Diagrams / Database Design

---

## 1. SDLC (Software Development Life Cycle)

The **Software Development Life Cycle** (SDLC) is a systematic, disciplined approach to producing high‑quality software that meets or exceeds customer expectations. It provides a framework that defines tasks performed at each step of the software development process, ensuring that all aspects—from initial planning to final deployment and ongoing maintenance—are handled in an organised manner. By following an SDLC model, teams can reduce risk, improve traceability, and deliver software on time and within budget.

### Phases of SDLC

1. **Requirement Gathering & Analysis**  
   This is the foundation of the entire project. Business analysts and product owners collaborate with stakeholders to elicit, document, and prioritise the functional and non‑functional requirements of the system. Techniques include interviews, surveys, workshops, and analysis of existing systems. The output is a comprehensive **Software Requirements Specification (SRS)** that serves as the contract between the development team and the client. This phase also includes feasibility studies to assess technical, economic, and legal viability.

2. **System Design**  
   The requirements are translated into a blueprint for the system. This phase is split into two levels:
   - **High‑Level Design (HLD)** : Defines the overall system architecture, technology stack, major modules, and data flow. It establishes the system boundaries and interactions with external systems.
   - **Low‑Level Design (LLD)** : Provides detailed specifications for each module, including class diagrams, interface contracts, database schemas (ER diagrams), and algorithms. The design documentation guides developers during implementation and serves as a reference for testers.

3. **Implementation / Coding**  
   Developers write actual source code based on the design documents. They follow coding standards, conduct peer code reviews, and use version control systems (e.g., Git) to manage changes. This phase is typically the longest and most resource‑intensive. Good practices include test‑driven development (TDD), continuous integration, and regular code inspections to catch defects early.

4. **Testing**  
   Once code is written, it undergoes various levels of testing to verify correctness, performance, and security.
   - **Unit Testing:** Individual components are tested in isolation (often by developers).
   - **Integration Testing:** Combined modules are tested to ensure they work together.
   - **System Testing:** The entire system is tested against the SRS.
   - **Acceptance Testing:** The client validates that the system meets their needs (UAT).  
   Bugs are logged, fixed, and re‑tested. The output is a test report and a defect log.

5. **Deployment**  
   The validated system is released to the production environment. Deployment may be done in stages (development → testing → staging → production) to minimise risk. Activities include setting up servers, migrating databases, and configuring monitoring tools. In modern DevOps cultures, deployment is automated using CI/CD pipelines.

6. **Maintenance**  
   After deployment, the system enters the maintenance phase, which covers bug fixes, performance enhancements, new feature additions, and security patches. This phase continues throughout the software’s operational life and often consumes the majority of project resources.

### Popular SDLC Models

| Model | Description |
|-------|-------------|
| **Waterfall**       | A linear, sequential approach where each phase must be fully completed before the next begins. Simple to understand but inflexible; changes are costly and late feedback is a major risk. |
| **V-Model**         | An extension of Waterfall that pairs each development phase with a corresponding testing phase (e.g., unit testing aligns with coding, acceptance testing with requirements). Emphasis is on verification and validation. |
| **Iterative**       | The system is built incrementally, with each iteration adding more functionality. This allows for early delivery of a basic working version and gradual refinement based on feedback. |
| **Spiral**          | Combines iterative development with risk analysis. Each loop of the spiral involves planning, risk assessment, engineering, and evaluation. Best suited for large, high‑risk projects. |
| **Agile**           | A family of iterative methodologies (Scrum, Kanban, XP) that value customer collaboration, responding to change, and delivering working software in short cycles (sprints). Agile is now the dominant approach for most projects due to its flexibility and client engagement. |
| **DevOps**          | Not just a development model but a cultural shift that integrates development and operations teams. It emphasises continuous integration, continuous delivery, infrastructure as code, and automated monitoring to shorten the feedback loop and accelerate release cycles. |

**Choosing a Model:** The selection depends on project size, complexity, requirements stability, team expertise, and customer involvement. For projects with well‑understood requirements, Waterfall may work; for most modern software, Agile or DevOps is preferred.

---

## 2. ORM (Object-Relational Mapping)

**Object‑Relational Mapping (ORM)** is a programming technique that allows developers to work with relational databases using the object‑oriented paradigm of their programming language. Instead of writing raw SQL statements, developers interact with objects that represent database tables, and the ORM framework automatically translates method calls and property accesses into SQL queries under the hood. This abstraction simplifies data access, reduces boilerplate code, and makes the application more maintainable.

### Benefits

- **Increased Productivity** – Developers can focus on business logic rather than tedious CRUD SQL. Common operations like create, read, update, and delete are handled with a few lines of code.
- **Type Safety** – Because the ORM uses strongly typed classes, many errors are caught at compile time, reducing runtime failures.
- **Maintainability** – Changes to the database schema can be managed with code migrations, and the domain models remain in sync with the database. Business rules can be centralised in the entity classes.
- **Database Agnosticism** – Switching from SQL Server to PostgreSQL or MySQL often requires only a change of the database provider and connection string; the ORM abstracts vendor‑specific SQL.
- **Security** – Modern ORMs use parameterised queries or prepared statements, which mitigate SQL injection attacks by default.
- **Integration with LINQ** – In .NET, ORMs like Entity Framework allow you to write query logic using LINQ, which is familiar and expressive.

### Drawbacks

- **Performance Overhead** – Automatically generated SQL may not be as efficient as hand‑optimised queries, especially for complex joins or bulk operations. However, many ORMs offer ways to execute raw SQL when needed.
- **Complexity** – For very intricate queries, the ORM’s abstraction can become cumbersome, and developers may struggle to tune the generated SQL.
- **Impedance Mismatch** – Object‑oriented concepts (inheritance, polymorphism, composition) do not map perfectly to relational tables. ORMs offer solutions (e.g., table‑per‑hierarchy), but they can add complexity.
- **Learning Curve** – Mastering an ORM requires understanding its configuration, change tracking, and lazy/eager loading, which can be daunting for beginners.

### Popular ORMs

- **.NET Ecosystem:**
  - **Entity Framework Core** – Microsoft’s flagship ORM, full‑featured, with excellent tooling and migration support.
  - **Dapper** – A micro‑ORM that focuses on performance and simplicity; it does not offer change tracking or migrations but gives you full control over SQL.
  - **NHibernate** – A mature, feature‑rich ORM ported from Java’s Hibernate; supports advanced mapping scenarios.
- **Java:** Hibernate, EclipseLink, JPA (standard API).
- **Python:** SQLAlchemy (flexible, powerful), Django ORM (tightly integrated with Django framework).
- **Node.js:** Sequelize (promise‑based), TypeORM (supports TypeScript), Prisma (next‑gen, type‑safe).
- **PHP:** Doctrine (for Symfony), Eloquent (Laravel).

---

## 3. Entity Framework (EF Core)

**Entity Framework Core** is a lightweight, extensible, cross‑platform ORM for .NET. It is the recommended data access technology for modern .NET applications, offering LINQ‑based queries, change tracking, automatic schema migrations, and support for multiple database providers. EF Core is open source and actively maintained by Microsoft.

### Key Concepts

- **DbContext** – The primary class that represents the database session. It coordinates the interaction between the domain entities and the underlying database. It also provides facilities for tracking changes, executing queries, and persisting updates.
- **DbSet<T>** – A property on the `DbContext` that exposes a collection of entities of type `T`. It acts as a gateway for querying and inserting/updating/deleting data. For example, `DbSet<Customer>` maps to the `Customers` table.
- **Entity** – A plain C# class (POCO) that represents a table in the database. Properties of the class correspond to columns. EF Core uses conventions (like `Id` for a primary key) but also allows explicit configuration via data annotations or Fluent API.
- **Migration** – A code‑based representation of schema changes. Migrations allow you to evolve the database schema as your model changes, without losing existing data. Each migration captures the differences between the previous and current model states, and includes `Up` and `Down` methods to apply or rollback changes.
- **Convention over Configuration** – EF Core assumes many defaults, such as table names being pluralised versions of `DbSet` property names, and key properties named `Id` or `<TypeName>Id`. You can override these conventions when needed.

### Typical Workflow (Code‑First)

1. **Install required NuGet packages**:
   - `Microsoft.EntityFrameworkCore`
   - Provider package (e.g., `Microsoft.EntityFrameworkCore.SqlServer`)
   - Tools package (optional, for migration commands):
     ```
     dotnet add package Microsoft.EntityFrameworkCore.Tools
     ```

2. **Define your entity classes** (POCOs):
   ```csharp
   public class Product
   {
       public int Id { get; set; }
       public string Name { get; set; }
       public decimal Price { get; set; }
   }
   ```

3. **Create a `DbContext` class**:
   ```csharp
   public class AppDbContext : DbContext
   {
       public DbSet<Product> Products { get; set; }
   
       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       {
           optionsBuilder.UseSqlServer("YourConnectionString");
       }
   }
   ```

4. **Create a migration**:
   ```
   dotnet ef migrations add InitialCreate
   ```
   This generates a `Migrations` folder with a migration class that contains `Up` and `Down` methods.

5. **Apply the migration to the database**:
   ```
   dotnet ef database update
   ```
   This executes the `Up` method to create the schema.

6. **Use the DbContext in your application**:
   ```csharp
   using (var db = new AppDbContext())
   {
       var product = new Product { Name = "Laptop", Price = 1200.50m };
       db.Products.Add(product);
       db.SaveChanges();   // INSERT executed
   
       var cheapProducts = db.Products.Where(p => p.Price < 500).ToList();
   }
   ```

### Approaches to Modelling

- **Code‑First** – Start with C# classes and generate the database via migrations. This is the most common and flexible approach.
- **Database‑First** – Reverse‑engineer the model from an existing database using the `Scaffold-DbContext` command. This is useful when you have a legacy database.
- **Model‑First** – (Legacy) Define the model using a visual designer (EDMX) and generate both the database and code. This approach is not supported in EF Core.

### Advanced Features

- **Lazy Loading** – Automatically load related data when you access navigation properties (requires proxy creation and virtual properties).
- **Eager Loading** – Load related data upfront using `Include()` and `ThenInclude()` to avoid multiple database round‑trips.
- **Change Tracking** – The context keeps track of changes made to entities; `SaveChanges()` automatically generates the appropriate `INSERT`, `UPDATE`, or `DELETE` statements.
- **Global Query Filters** – Define filters at the model level (e.g., soft‑delete) that are automatically applied to all queries.
- **Raw SQL Execution** – Use `FromSqlRaw()` for complex queries or `ExecuteSqlRaw()` for DML operations when LINQ is insufficient.

---

## 4. HTML (HyperText Markup Language)

**HTML** is the backbone of the World Wide Web. It is a markup language that defines the structure and semantics of web content. Browsers interpret HTML tags to render headings, paragraphs, lists, links, images, forms, and interactive elements. Since its inception, HTML has evolved through several versions, with HTML5 being the current standard that introduces semantic elements, multimedia support, and a host of new APIs.

### Basic Skeleton

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Document Title</title>
</head>
<body>
    <h1>Main Heading</h1>
    <p>This is a paragraph of text.</p>
</body>
</html>
```

- `<!DOCTYPE html>` declares the document type.
- `<html>` is the root element; the `lang` attribute specifies the language.
- `<head>` contains metadata, title, and linked resources (CSS, scripts).
- `<body>` contains all visible content.

### Common Tags and Their Uses

| Tag | Purpose |
|-----|---------|
| `<h1>` – `<h6>` | Defines headings, with `<h1>` being the highest level and `<h6>` the lowest. |
| `<p>` | Paragraph; adds vertical spacing between blocks of text. |
| `<a href="URL">` | Anchor (hyperlink) to navigate to another page or section. |
| `<img src="URL" alt="Description">` | Embeds an image; the `alt` attribute is critical for accessibility. |
| `<ul>`, `<ol>`, `<li>` | Unordered (bulleted) and ordered (numbered) lists with list items. |
| `<table>`, `<tr>`, `<td>`, `<th>` | Table structure; `tr` = table row, `td` = data cell, `th` = header cell. |
| `<form>`, `<input>`, `<button>`, `<textarea>`, `<select>` | Create interactive forms for user input. |
| `<div>` | A block‑level container used for grouping and styling. |
| `<span>` | An inline container for styling small portions of text. |
| `<header>`, `<nav>`, `<main>`, `<footer>`, `<section>`, `<article>` | Semantic elements that give meaning to the structure, improving SEO and accessibility. |

HTML5 introduced native form validation (e.g., `required`, `pattern`, `type="email"`), audio and video elements (`<audio>`, `<video>`), and the `<canvas>` element for dynamic graphics. It also provides a rich set of APIs for geolocation, local storage, drag‑and‑drop, and more.

---

## 5. CSS (Cascading Style Sheets)

**CSS** is the language used to describe the look and formatting of a document written in HTML. It separates content from presentation, enabling designers to apply consistent styles across an entire website. CSS rules are composed of selectors and declaration blocks, and they cascade based on specificity and source order.

### Ways to Apply CSS

1. **Inline styles** – Applied directly to an element using the `style` attribute. Overrides most other styles but is discouraged for maintenance reasons.
2. **Internal stylesheets** – Defined inside a `<style>` block in the `<head>` section. Useful for single‑page prototypes.
3. **External stylesheets** – Linked via `<link rel="stylesheet" href="styles.css">`. This is the recommended approach as it promotes reusability and separation of concerns.

### Selectors

CSS selectors target specific HTML elements to apply styles. Some common ones:

| Selector | Example | Description |
|----------|---------|-------------|
| Element (Type) | `p` | Selects all `<p>` elements. |
| Class | `.highlight` | Selects all elements with `class="highlight"`. |
| ID | `#main-header` | Selects the element with `id="main-header"` (should be unique). |
| Descendant | `div p` | Selects all `<p>` elements inside a `<div>` (any depth). |
| Child | `div > p` | Selects only `<p>` elements that are direct children of `<div>`. |
| Attribute | `input[type="text"]` | Selects `<input>` elements with `type="text"`. |
| Pseudo‑class | `a:hover` | Selects links when the user hovers over them. |
| Pseudo‑element | `p::first-line` | Selects the first line of a paragraph. |

### The Box Model

Every element in CSS is represented as a rectangular box. The box model comprises:
- **Content** – The actual text, image, or nested elements.
- **Padding** – Inner space between content and border.
- **Border** – The edge around the padding.
- **Margin** – Outer space separating the element from its neighbours.

Total width = content width + padding (left/right) + border (left/right) + margin (left/right). The `box-sizing` property can be set to `border-box` to include padding and border in the element's width, simplifying layout calculations.

### Layout Systems

- **Normal Flow** – Elements stack vertically (block) or horizontally (inline) according to their display value.
- **Flexbox** – A one‑dimensional layout model designed for distributing space along a single axis (row or column). It provides powerful alignment and ordering capabilities.
- **CSS Grid** – A two‑dimensional layout system that allows you to define rows and columns and place items into precise grid cells. Ideal for complex page layouts.
- **Positioning** – `position: relative/absolute/fixed/sticky` allows you to break out of the normal flow for overlays, sidebars, and sticky headers.

### Responsive Design

Responsive web design ensures that pages render well on various devices and screen sizes. Key techniques:
- **Fluid layouts** – Use percentages or `vw`/`vh` units instead of fixed pixels.
- **Media queries** – Apply different styles based on viewport width, height, or device features:
  ```css
  @media (max-width: 768px) {
      body { font-size: 14px; }
      .container { flex-direction: column; }
  }
  ```
- **Responsive images** – Use `srcset` and `sizes` attributes or `<picture>` element to serve appropriately sized images.
- **Mobile‑first design** – Start with styles for small screens and progressively enhance for larger ones.

Modern CSS also includes custom properties (variables), animations, transitions, and container queries, making it more powerful than ever.

---

## 6. ER Diagrams (Entity-Relationship Diagrams)

**Entity‑Relationship Diagrams** (ERDs) are a graphical representation of the entities within a system and the relationships between them. They are used extensively in database design to visualise the logical structure of the data, identify tables, attributes, primary and foreign keys, and the cardinalities of associations. ERDs help both technical and non‑technical stakeholders understand the data model before implementation.

### Components

| Symbol | Meaning |
|--------|---------|
| **Rectangle** | Represents an **entity** (a table in the relational model). Typically named with a singular noun (e.g., `Customer`, `Order`). |
| **Ellipse**   | Represents an **attribute** (a column). The underlined attribute denotes the primary key. |
| **Diamond**   | Represents a **relationship** between two or more entities. The diamond is labelled with a verb (e.g., `places`, `contains`). |
| **Lines**     | Connect entities to attributes, and connect entities to relationships. Line decorations indicate cardinality. |
| **Double rectangle** | Weak entity – depends on another entity for its existence. |
| **Double diamond**   | Identifying relationship – connects a weak entity to its owner. |

### Cardinality Notations

Cardinality defines how many instances of an entity participate in a relationship. Common notations include:

- **One‑to‑One (1:1)** – Each entity instance corresponds to exactly one instance of the other. Example: a `Person` has exactly one `Passport`.
- **One‑to‑Many (1:N)** – One entity instance can be associated with many instances of the other, but each instance of the other belongs to only one of the first. Example: a `Customer` can have many `Order`s, but each `Order` is placed by one `Customer`.
- **Many‑to‑Many (M:N)** – Many instances on both sides. Example: `Student` and `Course` – a student can enrol in many courses, and a course can have many students. This is resolved by introducing a junction (bridge) table.

### Example: Library Management System

Consider a database to manage books, members, and loans:

- **Entities:** `Book`, `Member`, `Loan`
- **Attributes:**
  - `Book`: `ISBN` (PK), `Title`, `Author`, `Publisher`, `Year`
  - `Member`: `MemberId` (PK), `Name`, `Email`, `JoinDate`
  - `Loan`: `LoanId` (PK), `BorrowDate`, `ReturnDate`
- **Relationships:**
  - A `Book` can be on many `Loan`s (historical) and a `Loan` refers to one `Book` → one‑to‑many from `Book` to `Loan`.
  - A `Member` can take many `Loan`s, and a `Loan` belongs to one `Member` → one‑to‑many from `Member` to `Loan`.
- The `Loan` entity also has foreign keys: `BookId` (FK) and `MemberId` (FK).

In an ER diagram, you would draw three rectangles (entities), two diamonds (`borrowed by` and `holds`), and connect them with lines annotated with `1` and `N` to indicate cardinality.

---

## 7. Database Design

**Database design** is the process of producing a detailed data model that accurately reflects the business domain, ensures data integrity, supports efficient querying, and allows for scalability. A well‑designed database is the foundation of any data‑driven application. The process typically follows a systematic approach from conceptual to physical implementation.

### Steps in Database Design

1. **Requirement Analysis**  
   Work with stakeholders to understand what data must be stored, what queries are most frequent, and what business rules apply. This includes identifying entities, their attributes, and the nature of relationships.

2. **Conceptual Design**  
   Create an Entity‑Relationship (ER) model that captures the high‑level structure. This is independent of any particular DBMS and focuses on the business view.

3. **Logical Design**  
   Transform the ER model into a relational schema (i.e., tables, columns, primary and foreign keys). This step also involves choosing data types and applying normalisation to reduce redundancy.

4. **Normalisation**  
   A formal technique to organise data to minimise redundancy and avoid update anomalies. The most common normal forms are:
   - **1NF (First Normal Form):** Ensure each column contains atomic (indivisible) values and there are no repeating groups.
   - **2NF (Second Normal Form):** Achieve 1NF and remove partial dependencies – every non‑key attribute must depend on the entire primary key (relevant for composite keys).
   - **3NF (Third Normal Form):** Achieve 2NF and remove transitive dependencies – no non‑key attribute should depend on another non‑key attribute.
   - **BCNF (Boyce‑Codd Normal Form):** A stricter version of 3NF where every determinant is a candidate key.
   - Higher normal forms (4NF, 5NF) exist but are rarely needed in practice.

5. **Physical Design**  
   Choose specific storage structures, data types (e.g., `VARCHAR(255)` vs. `TEXT`), indexing strategies, partitioning, and clustering. This phase is DBMS‑specific and involves performance considerations.

6. **Implementation**  
   Write the Data Definition Language (DDL) scripts to create tables, constraints, indexes, and views. For example:
   ```sql
   CREATE TABLE Customers (
       CustomerId INT PRIMARY KEY IDENTITY(1,1),
       Name NVARCHAR(100) NOT NULL,
       Email NVARCHAR(200) UNIQUE,
       CreatedAt DATETIME DEFAULT GETUTCDATE()
   );
   ```

7. **Tuning and Optimisation**  
   After deployment, monitor query performance, add or modify indexes, refactor queries, and sometimes denormalise (introduce redundant data) to meet performance goals. This is an ongoing process.

### Key Constraints

| Constraint | Purpose |
|------------|---------|
| `PRIMARY KEY`  | Uniquely identifies each row; automatically enforces uniqueness and non‑nullability. |
| `FOREIGN KEY`  | Ensures referential integrity – values in a column must match values in the primary key of another table (or be NULL). |
| `UNIQUE`       | Guarantees that all values in a column (or combination) are distinct. |
| `NOT NULL`     | Prevents NULL values from being inserted into the column. |
| `CHECK`        | Enforces a domain constraint, e.g., `Age >= 18` or `Status IN ('Active', 'Inactive')`. |
| `DEFAULT`      | Provides a default value when no explicit value is supplied during insertion. |

### Best Practices

- **Choose keys wisely:** Use **surrogate keys** (auto‑increment integers or GUIDs) as primary keys for most tables, as they are stable and simple. Use **natural keys** only when they are guaranteed to be unique and never change (e.g., ISO country codes).
- **Normalise to 3NF by default:** This reduces redundancy and maintains data integrity. Denormalise only when performance metrics clearly indicate a need, and do so carefully with documented trade‑offs.
- **Index strategically:** Create indexes on columns used frequently in `WHERE`, `JOIN`, and `ORDER BY` clauses. Avoid over‑indexing, as each index adds overhead to `INSERT`, `UPDATE`, and `DELETE`.
- **Use meaningful names:** Table names should be plural or singular (consistently), column names should be descriptive (e.g., `First_Name` instead of `FName`). Adopt a consistent naming convention (e.g., `snake_case`).
- **Document the schema:** Add comments to tables and columns explaining their purpose, especially for complex business rules.
- **Consider concurrency:** Use optimistic locking (e.g., a `RowVersion` column) or pessimistic locking to handle concurrent updates.
- **Plan for growth:** Choose data types that allow for future scaling (e.g., `NVARCHAR(MAX)` vs. fixed length) and consider partitioning for very large tables.

---

## Putting It All Together

A full‑stack .NET web application seamlessly integrates all these disciplines:

- **SDLC** provides the project framework that governs how the team works, from initial requirement gathering to post‑deployment maintenance.
- **HTML and CSS** shape the front‑end user interface, ensuring a responsive and accessible experience.
- **ER Diagrams** and **Database Design** provide the blueprint for storing and retrieving data in a relational database.
- **ORM (Entity Framework Core)** bridges the gap between the C# object model and the relational tables, allowing developers to work with data naturally while leveraging migrations to keep the schema in sync.
- The **business logic** in C# orchestrates the data flow, validates rules, and interacts with the database through EF Core.

Mastering these topics equips you with the fundamental knowledge required to design, build, and maintain robust enterprise applications. Each topic is deep, and continuous learning in each area is essential for professional growth.
