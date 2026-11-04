"""
EduPlatform AIService - Google Gemini LLM Provider (Primary Free-Tier)
Invokes Gemini 1.5 Flash with strict educational parameters.
"""

from typing import Dict, Any, Optional
from config import settings
from core.logger import logger
from core.exceptions import LLMQuotaExceededError

class GeminiProvider:
    def __init__(self):
        self.api_key = settings.GEMINI_API_KEY
        self.model_name = settings.GEMINI_MODEL
        self.client = None
        if self.api_key:
            try:
                from google import genai
                self.client = genai.Client(api_key=self.api_key)
            except Exception as e:
                logger.warning(f"Failed to initialize Google GenAI Client: {e}")

    def is_configured(self) -> bool:
        return bool(self.api_key and self.client)

    def generate(
        self,
        prompt: str,
        system_instruction: str,
        temperature: float = None,
        max_tokens: int = None
    ) -> Dict[str, Any]:
        if not self.is_configured():
            raise ValueError("Gemini API key is not configured.")

        temp = temperature if temperature is not None else settings.TEMPERATURE
        tokens = max_tokens if max_tokens is not None else settings.MAX_OUTPUT_TOKENS

        try:
            from google.genai import types
            response = self.client.models.generate_content(
                model=self.model_name,
                contents=prompt,
                config=types.GenerateContentConfig(
                    system_instruction=system_instruction,
                    temperature=temp,
                    max_output_tokens=tokens
                )
            )

            text = response.text or ""
            # Estimate tokens if usage metadata is present
            prompt_tokens = len(prompt.split()) + len(system_instruction.split())
            completion_tokens = len(text.split())

            return {
                "text": text,
                "model": self.model_name,
                "provider": "gemini",
                "prompt_tokens": prompt_tokens,
                "completion_tokens": completion_tokens,
                "total_tokens": prompt_tokens + completion_tokens
            }
        except Exception as ex:
            err_str = str(ex).lower()
            if "429" in err_str or "quota" in err_str or "resource_exhausted" in err_str:
                logger.warning(f"Gemini API free-tier quota/rate limit reached: {ex}")
                raise LLMQuotaExceededError("Gemini quota or rate limit exceeded", provider="gemini")
            logger.error(f"Gemini API call failed: {ex}")
            raise ex
