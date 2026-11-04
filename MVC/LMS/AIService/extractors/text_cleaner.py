"""
EduPlatform AIService - Text Normalization and Cleaning
Cleans raw document text, normalizes line breaks, and handles page artifacts.
"""

import re
import unicodedata

def clean_text(text: str) -> str:
    """
    Cleans raw extracted text:
    - Normalizes Unicode (NFKC)
    - Strips non-printable characters
    - Normalizes repeated whitespaces and blank lines
    - Removes hyphenated line breaks (e.g. 'para-\ngraph' -> 'paragraph')
    """
    if not text:
        return ""

    # Normalize unicode characters
    text = unicodedata.normalize("NFKC", text)

    # Rejoin hyphenated words split across lines
    text = re.sub(r"(\w+)-\n(\w+)", r"\1\2", text)

    # Convert carriage returns to newlines
    text = text.replace("\r\n", "\n").replace("\r", "\n")

    # Replace multiple spaces with a single space
    text = re.sub(r"[ \t]+", " ", text)

    # Replace 3 or more consecutive newlines with 2 newlines
    text = re.sub(r"\n{3,}", "\n\n", text)

    return text.strip()

def is_text_meaningful(text: str, min_chars: int = 20) -> bool:
    """Checks if extracted text contains sufficient alphanumeric content."""
    if not text:
        return False
    alnum_chars = sum(1 for c in text if c.isalnum())
    return alnum_chars >= min_chars
