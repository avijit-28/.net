"""
Summarizer Service for RAG AI Learning Assistant.
Produces executive summaries, key concept breakdowns, revision bullet points,
and study notes from course documents or lesson text.
"""
from typing import List, Dict, Any, Optional
from pydantic import BaseModel
from llm.provider_factory import LLMProviderFactory
from rag.prompt_templates import SystemPrompts
from core.logger import get_masked_logger

logger = get_masked_logger(__name__)

class SummaryResult(BaseModel):
    title: str
    summary_type: str  # "executive", "key_concepts", "revision_notes"
    summary_text: str
    key_points: List[str]
    suggested_revision_questions: List[str]
    tokens_used: int = 0
    provider_used: str = ""

class SummarizerService:
    def __init__(self):
        self.llm = LLMProviderFactory.get_provider()

    async def summarize_content(
        self,
        title: str,
        content_text: str,
        summary_type: str = "revision_notes"
    ) -> SummaryResult:
        """
        Generates structured revision notes or summary for content.
        """
        logger.info(f"Summarizing '{title}' with mode {summary_type}")

        prompt = f"""
Please generate a comprehensive, structured {summary_type.replace('_', ' ').upper()} for the following educational material.

TITLE: {title}

CONTENT:
{content_text[:8000]}

STRUCTURE YOUR RESPONSE AS FOLLOWS:
1. **Overview**: Concise 2-3 sentence executive synopsis.
2. **Key Concepts & Definitions**: Core concepts defined with bullet points.
3. **High-Yield Revision Points**: 5-7 bullet points for fast exam review.
4. **Self-Check Questions**: 3 reflective questions for students to test their understanding.
"""
        response_text, tokens, provider = await self.llm.generate(
            prompt=prompt,
            system_prompt=SystemPrompts.SUMMARIZATION,
            temperature=0.2
        )

        # Parse key points from response
        lines = response_text.splitlines()
        key_points = [line.strip().lstrip("*-•123456789. ") for line in lines if line.strip().startswith(("-", "*", "•"))][:7]

        return SummaryResult(
            title=title,
            summary_type=summary_type,
            summary_text=response_text,
            key_points=key_points,
            suggested_revision_questions=[
                f"How would you explain the core concept of {title} in your own words?",
                f"What are the main real-world applications of {title}?",
                f"What are the most common pitfalls to avoid when working with {title}?"
            ],
            tokens_used=tokens,
            provider_used=provider
        )
