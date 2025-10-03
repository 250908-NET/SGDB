# Core User Stories (MVP)

# US-001 — Register account (Must-Have)

As a Visitor, I want to register with a unique username so that I can log in and access my account features.

# Acceptance Criteria

Given I provide a unique username and valid password, when I submit, then I see a success message and can log in.

Given I provide a username that already exists, when I submit, then I see an error indicating the username is taken.

Password is validated against minimum rules (length, etc.).

US-002 — Login/Logout (Must-Have)
As a Visitor, I want to log in and log out so that I can securely access or end my session.
Acceptance Criteria

Valid credentials produce an authenticated session/JWT; invalid credentials return 401/invalid message.

Logout clears session/token.

US-003 — Manage played games list (CRUD) (Must-Have)
As a Gamer, I want to add, update, and remove games from my played list so that I can track what I’ve played.
Acceptance Criteria

I can add a game with fields: status (Playing/Completed/Abandoned), optional personal rating (1–5), and play notes.

I can update those fields; I can delete the entry.

Duplicate (same game twice) is prevented or merged.

US-004 — Preferred genres (Must-Have)
As a Gamer, I want to select preferred genres so that I can quickly find games that match my interests.
Acceptance Criteria

I can add/remove genres from my preferences.

The system stores multiple selections and reflects them in my profile.

US-005 — Reviews (CRUD) (Must-Have)
As a Gamer, I want to write, edit, and delete a review for a game I’ve played so that I can share my opinion.
Acceptance Criteria

I can create one review per game per user; creating a second prompts edit of the existing one.

I can edit or delete my own review only.

Attempting to review a game I haven’t “played” prompts an error or guidance.

US-006 — Browse, search, and filter games (Must-Have)
As a Gamer, I want to search by title and filter by genre/platform and sort by rating so that I can find games faster.
Acceptance Criteria

Title search returns partial matches.

Filters for genre and platform can be combined; sorting by average rating works.

Empty result shows a helpful “no results” state.

US-007 — View top-rated games (Must-Have)
As a Gamer, I want to see the highest-rated games so that I can pick something good to play.
Acceptance Criteria

A list sorted by community average rating (desc), with a default size (e.g., top 20).

Ties are consistently ordered (e.g., by rating then title).

US-008 — View other users’ profiles (Must-Have)
As a Gamer, I want to view other users’ public profiles so that I can see their lists and reviews.
Acceptance Criteria

I can search users by username.

I can view another user’s public played list, ratings, and reviews (without editing them).

Private fields (email, etc.) are not shown.

Admin Stories (MVP)

AD-001 — Manage games (CRUD) (Must-Have)
As an Admin, I want to add, edit, and delete games so that the catalog stays accurate.
Acceptance Criteria

Game fields include: title, release date, platforms, developer(s), publisher(s), genres.

Deleting a game with dependent data (reviews, lists) is blocked or requires a soft-delete strategy.

AD-002 — Manage genres (CRUD) (Must-Have)
As an Admin, I want to add, edit, and delete genres so that users can tag and filter correctly.
Acceptance Criteria

Genre names must be unique; attempts to create duplicates are rejected.

Deleting a genre updates or prevents deletion if still in use (decide policy).

AD-003 — Manage platforms (CRUD) (Must-Have)
As an Admin, I want to add, edit, and delete platforms so that new game titles can reference correct platforms.
Acceptance Criteria

Platform names unique; cannot delete if referenced (or apply soft-delete).

AD-004 — Manage companies (CRUD) (Must-Have)
As an Admin, I want to add, edit, and delete companies so that developers and publishers are correctly assigned.
Acceptance Criteria

Company names unique; roles can be Developer, Publisher, or both.

Cannot delete if in use (or soft-delete).

Stretch Goals (Could-Have)

ST-001 — Review moderation
As an Admin, I want to hide or remove reviews that violate policy so that content stays appropriate.
Acceptance Criteria

Admin can mark a review as hidden; hidden reviews are not shown to normal users.

Actions are audited (who/when).

ST-002 — User management
As an Admin, I want to deactivate or change roles for users so that the database remains moderated.
Acceptance Criteria

Deactivated users cannot log in; their content is preserved but marked.

Role changes require admin privileges.

ST-003 — Game recommendations
As a Gamer, I want to recommend a game to another user so that my friends can discover titles I like.
Acceptance Criteria

I can send a recommendation to a chosen user with a short note.

The recipient sees a notification or inbox item.

ST-004 — Wishlist (CRUD)
As a Gamer, I want to manage a wishlist so that I can track games I want to play later.
Acceptance Criteria

Add/remove games to wishlist; item cannot duplicate within wishlist.

Wishlist is separate from played list.

ST-005 — Profile image upload
As a Gamer, I want to upload a profile image so that I can personalize my page.
Acceptance Criteria

Accept common formats (jpg/png), max size limit, and basic validation.

Display the image on the user’s profile; fallback to default avatar.

Notes & Constraints (useful for your overseer)

Authorization: Admin endpoints require admin role; user-owned resources enforce “only owner can edit/delete.”

Uniqueness: Enforce at DB level (unique indexes) for usernames, genres, platforms, companies, and (userId, gameId) pair for reviews/played entries.

One review per user per game: enforce via unique constraint on (UserId, GameId) in Reviews.

Soft-delete vs hard-delete: Decide policy for games/genres/platforms/companies with dependencies (soft-delete recommended).

Non-functional: Search returns in < 500 ms for typical queries on seed dataset; list endpoints paginated.

How to present these (quick tips)

Keep the story sentence 1 line.

Add 3–4 crisp acceptance criteria max, in Given/When/Then style if your overseer prefers BDD.

Tag each with Priority (Must/Could), Owner, and Estimate (story points or t-shirt size).

If you want, I can convert any subset of these into strict Gherkin (Given/When/Then) or map each story to API endpoints + DB constraints for your spec.
