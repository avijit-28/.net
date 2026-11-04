"""
EduPlatform AIService - Custom Domain Exceptions
"""

class AIServiceException(Exception):
    """Base exception for all AI Service errors."""
    def __init__(self, message: str, status_code: int = 500, details: dict = None):
        super().__init__(message)
        self.message = message
        self.status_code = status_code
        self.details = details or {}

class AuthenticationError(AIServiceException):
    def __init__(self, message: str = "Invalid or missing internal service credentials"):
        super().__init__(message, status_code=401)

class AuthorizationError(AIServiceException):
    def __init__(self, message: str = "Access to requested course material is forbidden"):
        super().__init__(message, status_code=403)

class DocumentExtractionError(AIServiceException):
    def __init__(self, message: str, details: dict = None):
        super().__init__(message, status_code=422, details=details)

class VectorStoreError(AIServiceException):
    def __init__(self, message: str, details: dict = None):
        super().__init__(message, status_code=503, details=details)

class LLMQuotaExceededError(AIServiceException):
    def __init__(self, message: str = "LLM provider rate limit or quota exceeded", provider: str = "unknown"):
        super().__init__(message, status_code=429, details={"provider": provider})
