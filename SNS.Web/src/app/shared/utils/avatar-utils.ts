/**
 * Generates up to two initials from a name (user full name or community name).
 * - Multi-word: First character of the first two words (e.g., "Ali Mallouhe" -> "AM", "Syrian Developers Network" -> "SD").
 * - Single word: First two characters (e.g., "Ali" -> "AL", "Developers" -> "DE").
 * - Single character: That single character.
 * - Safely handles empty/null, multiple spaces, Arabic, English, RTL/LTR.
 */
export function getInitials(name?: string | null): string {
    if (!name) return '';
    const trimmed = name.trim();
    if (!trimmed) return '';

    const words = trimmed.split(/\s+/).filter(w => w.length > 0);
    if (words.length === 0) return '';

    if (words.length === 1) {
        const chars = Array.from(words[0]);
        return chars.slice(0, 2).join('').toUpperCase();
    }

    const firstChar = Array.from(words[0])[0];
    const secondChar = Array.from(words[1])[0];
    return (firstChar + secondChar).toUpperCase();
}
