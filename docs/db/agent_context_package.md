# Agent Context Package — `testmaker_v2` Database
<!-- Paste this block into your agent's system prompt -->

---

## 1. Database Overview

**Database:** `testmaker_v2` (MySQL)
**Purpose:** A test/exam creation platform. Schools create tests from a bank of questions. Questions have types, difficulty levels, and subject tags. Tests are assembled by mapping questions to a test record, with optional sub-question nesting.

---

## 2. Domain Map

| Domain | Tables | Role |
|---|---|---|
| **Question** | `question_details`, `question_images`, `question_subquestion_map` | Core question bank and media |
| **Test** | `test`, `test_question_map` | Test assembly and structure |
| **Reference** | `question_type`, `question_difficulty`, `subject`, `class`, `school`, `test_type` | Lookup / enum tables — read-only, rarely mutated |

---

## 3. Table Schemas

### `question_details` — [Question domain] — SOURCE OF TRUTH for questions
```
  id               varchar(36)  [PK]
  question_type_id varchar(36)  [FK]  → question_type.id  NOT NULL
  subject_id       varchar(36)  [FK]  → subject.id         NOT NULL
  difficulty       varchar(36)  [FK]  → question_difficulty.id  NOT NULL
  marks            int                                      NOT NULL
  content?         longtext     -- main question text (nullable: some questions are image-only)
  mcq?             json         -- populated only when question_type = MCQ
  match_a?         json         -- populated only when question_type = Match the Following
  match_b?         json         -- populated only when question_type = Match the Following
  fib_words?       json         -- populated only when question_type = Fill in the Blank
  reason?          longtext     -- populated only when question_type = Assertion-Reason
  assertion?       longtext     -- populated only when question_type = Assertion-Reason
  created_on       datetime
  updated_on       datetime
```
> ⚠️ **Agent note — conditional JSON columns:** `mcq`, `match_a`, `match_b`, `fib_words`, `reason`, `assertion` are conditionally populated based on `question_type_id`. Always JOIN `question_type` to determine which column is relevant. Do NOT read all JSON columns — wrong ones return NULL silently.


---

### `question_images` — [Question domain] — Images attached to a question
```
  id              varchar(36)  [PK]
  question_id     varchar(36)  [FK] → question_details.id  NOT NULL
  image_position  int          -- display order of image within the question
  image_name      varchar(50)  -- filename reference
  created_on      datetime
  updated_on      datetime
```
> One question can have multiple images (1:N). Always ORDER BY `image_position` when fetching.

---

### `question_subquestion_map` — [Question domain] — BRIDGE: parent question ↔ sub-question within a test
```
  id                varchar(36)  [PK]
  test_id           varchar(36)  [FK] → test.id                          NOT NULL
  question_id       varchar(36)  [FK] → question_details.id  (parent)    NOT NULL
  subquestion_id    varchar(36)  [FK] → question_details.id  (child)     NOT NULL
  subquestion_number  int        -- ordering of sub-questions under the parent
  created_on        datetime
  updated_on        datetime
```
> ⚠️ Both `question_id` and `subquestion_id` reference `question_details` — use table aliases (`parent`, `child`) when joining both in the same query. Mapping is scoped per `test_id`.

---

### `test` — [Test domain] — SOURCE OF TRUTH for tests
```
  id             varchar(36)  [PK]
  file_name      longtext                                       NOT NULL
  school_id      varchar(36)  [FK] → school.id                 NOT NULL
  class_id       varchar(36)  [FK] → class.id                  NOT NULL
  subject_id     varchar(36)  [FK] → subject.id                NOT NULL
  test_type_id   varchar(36)  [FK] → test_type.id              NOT NULL
  sections?      json         -- array of question position numbers where new sections begin
  time_duration  int          -- in minutes                     NOT NULL
  maximum_marks  int                                            NOT NULL
  created_on     datetime
  updated_on     datetime
```
> All four FK columns are NOT NULL — use INNER JOIN for all. `sections` is nullable JSON; parse as array, not scalar.

---

### `test_question_map` — [Test domain] — BRIDGE: test ↔ question
```
  id                varchar(36)  [PK]
  test_id           varchar(36)  [FK] → test.id                NOT NULL
  question_id       varchar(36)  [FK] → question_details.id    NOT NULL
  question_position int          -- display order within the test
  created_on        datetime
  updated_on        datetime
```
> Always ORDER BY `question_position` when rendering a test. Never rely on insertion order.

---

### `question_type` — [Reference] — Enum: question format
```
  id          varchar(36)  [PK]  -- UUID (changed from int)
  type        varchar(50)  -- e.g. MCQ, FIB, Match, Assertion-Reason, Short Answer
  created_on  datetime
  updated_on  datetime
```
> **Agent note:** PK is `varchar(36)` UUID — do NOT treat as integer. DB-level FK constraint `question_details_question_type_FK` is enforced.

---

### `question_difficulty` — [Reference] — Enum: difficulty level
```
  id          varchar(36)  [PK]
  level       varchar(50)  -- e.g. Easy, Medium, Hard
  created_on  datetime
  updated_on  datetime
```

---

### `subject` — [Reference] — Subject / topic
```
  id          varchar(36)  [PK]
  name        varchar(50)
  created_on  datetime
  updated_on  datetime
```

---

### `class` — [Reference] — School class / grade
```
  id          varchar(36)  [PK]
  class_name  varchar(100)
  created_on  datetime
  updated_on  datetime
```

---

### `school` — [Reference] — School record
```
  id             varchar(36)  [PK]
  name           varchar(50)
  logo_filename? text         (nullable)
  created_on     datetime
  updated_on     datetime
```

---

### `test_type` — [Reference] — Enum: test category
```
  id          varchar(36)  [PK]
  type        varchar(20)  -- e.g. Unit Test, Mid-Term, Final
  created_on  datetime
  updated_on  datetime
```

---

## 4. Foreign Key Map (Full)

```
question_details.difficulty        → question_difficulty.id
question_details.subject_id        → subject.id

question_images.question_id        → question_details.id

question_subquestion_map.question_id    → question_details.id  (parent)
question_subquestion_map.subquestion_id → question_details.id  (child)
question_subquestion_map.test_id        → test.id

test.school_id    → school.id
test.class_id     → class.id
test.subject_id   → subject.id
test.test_type_id → test_type.id

test_question_map.test_id      → test.id
test_question_map.question_id  → question_details.id

question_details.question_type_id  → question_type.id
```

---

## 5. Canonical Traversal Paths

**A. Fetch all questions in a test (ordered):**
```sql
SELECT qd.*, tqm.question_position
FROM test t
INNER JOIN test_question_map tqm ON tqm.test_id = t.id
INNER JOIN question_details qd   ON qd.id = tqm.question_id
WHERE t.id = ?
ORDER BY tqm.question_position;
```

**B. Fetch a question with full metadata (type + difficulty + subject):**
```sql
SELECT
  qd.*,
  qt.type        AS question_type,
  qdi.level      AS difficulty_level,
  s.name         AS subject_name
FROM question_details qd
INNER JOIN question_type       qt  ON qt.id  = qd.question_type_id
INNER JOIN question_difficulty qdi ON qdi.id = qd.difficulty
INNER JOIN subject             s   ON s.id   = qd.subject_id
WHERE qd.id = ?;
```


**C. Fetch a test with full context (school + class + subject + type):**
```sql
SELECT
  t.*,
  sc.name        AS school_name,
  cl.class_name,
  su.name        AS subject_name,
  tt.type        AS test_type
FROM test t
INNER JOIN school    sc ON sc.id = t.school_id
INNER JOIN class     cl ON cl.id = t.class_id
INNER JOIN subject   su ON su.id = t.subject_id
INNER JOIN test_type tt ON tt.id = t.test_type_id
WHERE t.id = ?;
```

**D. Fetch sub-questions of a parent question within a test:**
```sql
SELECT
  child.*,
  qsm.subquestion_number
FROM question_subquestion_map qsm
INNER JOIN question_details AS child ON child.id = qsm.subquestion_id
WHERE qsm.test_id     = ?
  AND qsm.question_id = ?
ORDER BY qsm.subquestion_number;
```

**E. Fetch images for a question:**
```sql
SELECT * FROM question_images
WHERE question_id = ?
ORDER BY image_position;
```

**F. Fetch all tests for a school, filtered by class and subject:**
```sql
SELECT t.*, cl.class_name, su.name AS subject_name, tt.type AS test_type
FROM test t
INNER JOIN class     cl ON cl.id = t.class_id
INNER JOIN subject   su ON su.id = t.subject_id
INNER JOIN test_type tt ON tt.id = t.test_type_id
WHERE t.school_id  = ?
  AND t.class_id   = ?
  AND t.subject_id = ?;
```

---

## 6. Query Generation Rules

1. **All FKs in `test` are NOT NULL** — always use INNER JOIN for `school`, `class`, `subject`, `test_type`.
2. **All FKs in `question_details` are NOT NULL** — use INNER JOIN for `question_type`, `question_difficulty`, and `subject`. All three are DB-enforced.
4. **All PKs across all tables are now `varchar(36)` UUIDs** — including `question_type.id`. Never compare or pass these as integers.
5. **Check `question_type` before reading JSON columns.** Only read `mcq` for MCQ, `match_a`/`match_b` for Match, `fib_words` for FIB, `reason`/`assertion` for Assertion-Reason.
6. **Order by position columns**, not `id` or `created_on`:
   - Questions in a test → `test_question_map.question_position`
   - Sub-questions → `question_subquestion_map.subquestion_number`
   - Images → `question_images.image_position`
7. **`question_details.content` is nullable** — do not use it as a proxy for question existence. Image-only questions have `content = NULL`.
8. **`test.sections` is a nullable JSON array** — use `JSON_EXTRACT` or app-layer parsing; never treat as scalar.
9. **Do not mutate reference tables** (`question_type`, `question_difficulty`, `subject`, `class`, `school`, `test_type`) unless the task is an explicit admin/seed operation.

---

## 7. Data Validation Rules

**When inserting `question_details`:**
- `question_type_id` must match a `varchar(36)` UUID in `question_type.id` — validate in app code, DB will not enforce
- `subject_id` must exist in `subject.id` (DB enforced)
- `difficulty` must exist in `question_difficulty.id` (DB enforced)
- Populate only the JSON column matching the question type; leave others NULL
- `content` may be NULL only if at least one image exists in `question_images` for that `question_details.id`

**When inserting `test`:**
- `school_id`, `class_id`, `subject_id`, `test_type_id` are all required NOT NULL
- `sections` if provided must be a valid JSON array of integers

**When inserting `test_question_map`:**
- `question_position` must be unique per `test_id`

**When inserting `question_subquestion_map`:**
- `question_id` ≠ `subquestion_id`
- `subquestion_number` must be unique per (`test_id`, `question_id`) pair

---

## 8. Schema Changelog

| Version | What changed | Old | New |
|---|---|---|---|
| v1 → v2 | `class` PK | `class_number` int | `id` varchar(36) UUID |
| v1 → v2 | `class` display column | `class_roman` | `class_name` varchar(100) |
| v1 → v2 | `test` FK to class | `class_number` nullable | `class_id` NOT NULL |
| v1 → v2 | `test` FK nullability | `school_id`, `subject_id`, `test_type_id` nullable | all NOT NULL |
| v2 → v3 | `question_type.id` type | `int` | `varchar(36)` UUID |
| v2 → v3 | `question_details.question_type_id` type | `int` | `varchar(36)` |
| v2 → v3 | `question_details_question_type_FK` | DB-enforced constraint | **removed** — app must validate |
| v3 → v4 | `question_details_question_type_FK` | removed | **restored** — DB-enforced again |

---

## 9. Schema Diagram (Text)

```
[school] ──────────────────────┐
[class] ───────────────────────┤
[subject] ─────────────────────┼──(INNER)──→ [test] ──→ [test_question_map] ──→ [question_details]
[test_type] ───────────────────┘                   └──→ [question_subquestion_map]──→ (self: parent/child)
                                                                    ├──(INNER)──→ [question_type]       (ref)
                                                                    ├──(INNER)──→ [question_difficulty] (ref)
                                                                    ├──(INNER)──→ [subject]             (ref)
                                                                    └──────────→ [question_images]

```