"""
EduPlatform AIService - Groq LLM Provider (Failover Free-Tier)
Invokes Groq Llama 3 / Mixtral for high-speed fallback when primary quotas are met.
"""

from typing import Dict, Any, Optional
from config import settings
from core.logger import logger
from core.exceptions import LLMQuotaExceededError

class GroqProvider:
    def __init__(self):
        self.api_key = settings.GROQ_API_KEY
        self.model_name = settings.GROQ_MODEL
        self.client = None
        if self.api_key:
            try:
                from groq import Groq
                self.client = Groq(api_key=self.api_key)
            except Exception as e:
                logger.warning(f"Failed to initialize Groq Client: {e}")

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
            raise ValueError("Groq API key is not configured.")

        temp = temperature if temperature is not None else settings.TEMPERATURE
        tokens = max_tokens if max_tokens is not None else settings.MAX_OUTPUT_TOKENS

        try:
            chat_completion = self.client.chat.completions.create(
                messages=[
                    {"role": "system", "content": system_instruction},
                    {"role": "user", "content": prompt}
                ],
                model=self.model_name,
                temperature=temp,
                max_tokens=tokens
            )

            choice = chat_completion.choices[0]
            text = choice.message.content or ""
            usage = chat_completion.usage

            prompt_tokens = usage.prompt_tokens if usage else len(prompt.split())
            completion_tokens = usage.completion_tokens if usage else len(text.split())

            return {
                "text": text,
                "model": self.model_name,
                "provider": "groq",
                "prompt_tokens": prompt_tokens,
                "completion_tokens": completion_tokens,
                "total_tokens": prompt_tokens + completion_tokens
            }
        except Exception as ex:
            err_str = str(ex).lower()
            if "429" in err_str or "rate limit" in err_str or "tokens per day" in err_str:
                logger.warning(f"Groq API free-tier quota/rate limit reached: {ex}")
                raise LLMQuotaExceededError("Groq quota or rate limit exceeded", provider="groq")
            logger.error(f"Groq API call failed: {ex}")
            raise ex
