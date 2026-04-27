# Expense Tracker - Development Roadmap

-- loading?? 
-- why isn't the caching working properly?
-- should probably design a single query that retrives accounts, categories
and tags immediately not all three seperately

-- probably should add ability for budget to span all accounts
-- need to revise category card
-- need to add loading to dashboard graphs



gameplan for md3

🔥 High ROI (Quick Wins, Big Impact)

    Update the Sidebar Active State: MD3 ditched the full-width rectangular highlights for navigation drawers. Instead, the active item (like "Accounts" in your screenshots) should use a pill-shaped (fully rounded) indicator that wraps just the text and icon, rather than stretching edge-to-edge.

    Modernize the Top App Bar: Your current Top App Bar is a solid, heavy block of primary purple. MD3 generally moves away from heavy colored headers unless necessary. Instead, use a "Surface" color (matching or slightly tinted from the background) and let the content below it breathe. The app will instantly feel lighter and more spacious.

    Clean Up the Data Grid (Your Idea): Implement that ⋮ (more_vert) icon for the edit/delete actions on the transactions table. While you're at it, ensure the table rows have plenty of vertical padding. MD3 loves breathing room.

⚡ Medium ROI (Noticeable Polish, Moderate Effort)

    Relocate the Primary Action (+ ADD): Currently, your "+ ADD" button is in the top header. In MD3, primary page actions are usually handled by a Floating Action Button (FAB) in the bottom right corner, or placed directly next to the page title ("Accounts" or "Transactions") within the main content area. A prominent FAB would look fantastic here.

    Define Card Variants: Your "Cash" and "Bank" accounts look like standard cards. MD3 strictly defines three card types: Elevated (uses shadow), Filled (uses a slight surface tint with no shadow), and Outlined (transparent with a subtle border). Pick one distinct style for your account cards to make them pop against the background. A "Filled" card with a slightly darker surface tint than the background looks very modern.

    Standardize the Chips (Tags): Your tags ("Work Related", "Emergency") look good, but ensure they follow MD3 chip guidelines. They should have a slightly lower height, rounded corners (often 8px instead of fully pill-shaped in some MD3 contexts, though pill is still acceptable), and a clear distinction between the background color and the text.

🛠️ Lower ROI (Subtle Details, Higher Effort)

    Tonal Color Palettes: MD3 is built entirely around dynamic, tonal color palettes (Primary, Secondary, Tertiary, Surface, Error, etc., and their "Container" variants). Mapping your MudBlazor theme to strictly use these tonal pairings (e.g., a Primary button uses Primary background and OnPrimary text) requires some configuration but yields beautiful dark/light mode transitions.

    Typography Scale: MD3 updated its type scale (Display, Headline, Title, Label, Body). Ensuring your page headers ("Accounts", "Transactions") map to the correct Headline size, and your table data maps to Body or Label sizes, will make the app feel professionally designed.