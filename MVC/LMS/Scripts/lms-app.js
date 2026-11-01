/**
 * EduPlatform LMS - Master Application JavaScript Engine
 * Handles Celebrations (Confetti), Classroom Theater & Scratchpad, Quiz Navigator,
 * Dynamic Filtering, Certificate Tools, and Accessibility Shortcuts.
 */

(function (window, document, $) {
    'use strict';

    var LMS = window.LMS = window.LMS || {};

    /* ==========================================================================
       0. Currency Formatting Helpers (Indian Rupee - INR / Rs)
       ========================================================================== */
    LMS.currencySymbol = '\u20B9';
    LMS.currencyCode = 'INR';
    LMS.formatRupee = function (amount) {
        var num = parseFloat(amount) || 0;
        return LMS.currencySymbol + num.toLocaleString('en-IN', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
    };
    LMS.formatCurrency = function (amount) {
        return LMS.formatRupee(amount);
    };

    /* ==========================================================================
       1. Toast Notification System
       ========================================================================== */
    LMS.toast = function (message, type, duration) {
        type = type || 'info';
        duration = duration || 3500;

        var container = document.getElementById('lmsToastContainer');
        if (!container) {
            container = document.createElement('div');
            container.id = 'lmsToastContainer';
            document.body.appendChild(container);
        }

        var iconMap = {
            success: 'bi-check-circle-fill text-success',
            warning: 'bi-exclamation-triangle-fill text-warning',
            danger: 'bi-x-circle-fill text-danger',
            info: 'bi-info-circle-fill text-primary'
        };

        var toast = document.createElement('div');
        toast.className = 'lms-toast toast-' + type;
        toast.innerHTML = '<i class="bi ' + (iconMap[type] || iconMap.info) + ' fs-5"></i><span>' + message + '</span>';

        container.appendChild(toast);

        setTimeout(function () {
            toast.style.transition = 'opacity 0.3s ease, transform 0.3s ease';
            toast.style.opacity = '0';
            toast.style.transform = 'translateY(15px) scale(0.95)';
            setTimeout(function () {
                if (toast.parentNode) {
                    toast.parentNode.removeChild(toast);
                }
            }, 300);
        }, duration);
    };

    /* ==========================================================================
       2. Zero-Dependency Canvas Confetti Engine
       ========================================================================== */
    LMS.confetti = function (options) {
        options = options || {};
        var particleCount = options.particleCount || 75;
        var canvas = document.getElementById('lmsConfettiCanvas');
        if (!canvas) {
            canvas = document.createElement('canvas');
            canvas.id = 'lmsConfettiCanvas';
            canvas.style.position = 'fixed';
            canvas.style.top = '0';
            canvas.style.left = '0';
            canvas.style.width = '100vw';
            canvas.style.height = '100vh';
            canvas.style.pointerEvents = 'none';
            canvas.style.zIndex = '99999';
            document.body.appendChild(canvas);
        }

        var ctx = canvas.getContext('2d');
        var width = canvas.width = window.innerWidth;
        var height = canvas.height = window.innerHeight;

        var particles = [];
        var colors = ['#4f46e5', '#06b6d4', '#10b981', '#f59e0b', '#ec4899', '#8b5cf6'];

        for (var i = 0; i < particleCount; i++) {
            particles.push({
                x: width * (0.3 + Math.random() * 0.4),
                y: height * 0.6,
                vx: (Math.random() - 0.5) * 16,
                vy: -(Math.random() * 14 + 10),
                size: Math.random() * 8 + 4,
                color: colors[Math.floor(Math.random() * colors.length)],
                rotation: Math.random() * 360,
                rotationSpeed: (Math.random() - 0.5) * 12,
                opacity: 1,
                decay: Math.random() * 0.015 + 0.01
            });
        }

        var animationFrame;
        function update() {
            ctx.clearRect(0, 0, width, height);
            var active = 0;

            for (var i = 0; i < particles.length; i++) {
                var p = particles[i];
                if (p.opacity <= 0) continue;

                p.x += p.vx;
                p.y += p.vy;
                p.vy += 0.4; // gravity
                p.rotation += p.rotationSpeed;
                p.opacity -= p.decay;
                active++;

                ctx.save();
                ctx.globalAlpha = Math.max(0, p.opacity);
                ctx.translate(p.x, p.y);
                ctx.rotate((p.rotation * Math.PI) / 180);
                ctx.fillStyle = p.color;
                ctx.fillRect(-p.size / 2, -p.size / 2, p.size, p.size * 1.4);
                ctx.restore();
            }

            if (active > 0) {
                animationFrame = requestAnimationFrame(update);
            } else {
                ctx.clearRect(0, 0, width, height);
                cancelAnimationFrame(animationFrame);
            }
        }

        update();
    };

    /* ==========================================================================
       3. Classroom Learning Engine (Theater Mode, Scratchpad, Keyboard Nav)
       ========================================================================== */
    LMS.initClassroom = function (options) {
        options = options || {};
        var lessonId = options.lessonId;
        var courseId = options.courseId;

        // Focus Mode / Distraction-Free Theater Toggle
        var $classroom = $('.classroom-container');
        var $body = $('body');
        var $focusBtn = $('#btnToggleTheater');
        var $sidebarToggle = $('#btnToggleSidebarInFocus');
        var $sidebar = $('.classroom-sidebar');

        function setFocusMode(active) {
            if (active) {
                $classroom.addClass('focus-mode').removeClass('theater-mode');
                $body.addClass('focus-mode-active').removeClass('theater-mode-active');
                $focusBtn.addClass('btn-primary text-white').removeClass('btn-outline-light')
                         .html('<i class="bi bi-fullscreen-exit me-1"></i> Exit Focus Mode');
                $sidebarToggle.removeClass('d-none');
                localStorage.setItem('lms_focus_mode', 'true');
            } else {
                $classroom.removeClass('focus-mode theater-mode');
                $body.removeClass('focus-mode-active theater-mode-active');
                $sidebar.removeClass('show-in-focus');
                $focusBtn.removeClass('btn-primary text-white').addClass('btn-outline-light')
                         .html('<i class="bi bi-moon-stars me-1"></i> Focus Mode');
                $sidebarToggle.addClass('d-none');
                localStorage.setItem('lms_focus_mode', 'false');
            }
        }

        // Restore saved preference
        var savedFocus = localStorage.getItem('lms_focus_mode') === 'true';
        if (savedFocus) {
            setFocusMode(true);
        }

        $focusBtn.on('click', function () {
            var isCurrentlyFocus = $classroom.hasClass('focus-mode');
            setFocusMode(!isCurrentlyFocus);
            if (!isCurrentlyFocus) {
                LMS.toast('Distraction-Free Focus Mode activated (Press Esc to exit)', 'info');
            } else {
                LMS.toast('Returned to Normal Classroom View', 'info');
            }
        });

        // Syllabus drawer toggle when inside Focus Mode
        $sidebarToggle.on('click', function () {
            $sidebar.toggleClass('show-in-focus');
        });

        // Esc key exits Focus Mode
        $(document).on('keydown', function (e) {
            if (e.key === 'Escape' && $classroom.hasClass('focus-mode')) {
                setFocusMode(false);
                LMS.toast('Exited Focus Mode', 'info');
            }
        });

        // Student Local Scratchpad
        if (lessonId) {
            var storageKey = 'lms_scratchpad_' + courseId + '_' + lessonId;
            var $textarea = $('#lessonScratchpad');
            var $saveStatus = $('#scratchpadStatus');
            var $charCount = $('#scratchpadCharCount');

            function updateCharCount() {
                var len = ($textarea.val() || '').length;
                if ($charCount.length) {
                    $charCount.text(len + ' characters');
                }
            }

            if ($textarea.length) {
                var savedNotes = localStorage.getItem(storageKey);
                if (savedNotes) {
                    $textarea.val(savedNotes);
                    updateCharCount();
                }

                var debounceTimer;
                $textarea.on('input', function () {
                    updateCharCount();
                    $saveStatus.html('<i class="bi bi-arrow-repeat spin"></i> Saving...');
                    clearTimeout(debounceTimer);
                    debounceTimer = setTimeout(function () {
                        localStorage.setItem(storageKey, $textarea.val());
                        $saveStatus.html('<i class="bi bi-cloud-check-fill text-success"></i> Autosaved locally');
                    }, 400);
                });

                // Copy notes to clipboard
                $('#btnCopyNotes').on('click', function () {
                    var notes = $textarea.val();
                    if (!notes || !notes.trim()) {
                        LMS.toast('Scratchpad is empty', 'warning');
                        return;
                    }
                    if (navigator.clipboard) {
                        navigator.clipboard.writeText(notes).then(function () {
                            LMS.toast('Notes copied to clipboard!', 'success');
                        });
                    } else {
                        $textarea.select();
                        document.execCommand('copy');
                        LMS.toast('Notes copied to clipboard!', 'success');
                    }
                });

                // Clear notes for this lesson
                $('#btnClearNotes').on('click', function () {
                    if (confirm('Clear scratchpad notes for this lesson?')) {
                        $textarea.val('');
                        localStorage.removeItem(storageKey);
                        updateCharCount();
                        $saveStatus.html('<i class="bi bi-cloud-check-fill text-success"></i> Cleared');
                        LMS.toast('Scratchpad cleared', 'info');
                    }
                });
            }
        }

        // Keyboard Shortcuts (Alt+N = Next, Alt+P = Prev, Alt+C = Complete, Alt+F = Focus)
        $(document).on('keydown', function (e) {
            if ($(e.target).is('input, textarea, select')) return;

            if (e.altKey && (e.key === 'n' || e.key === 'N')) {
                var $next = $('#btnNextLesson');
                if ($next.length && $next.attr('href')) {
                    window.location.href = $next.attr('href');
                }
            } else if (e.altKey && (e.key === 'p' || e.key === 'P')) {
                var $prev = $('#btnPrevLesson');
                if ($prev.length && $prev.attr('href')) {
                    window.location.href = $prev.attr('href');
                }
            } else if (e.altKey && (e.key === 'c' || e.key === 'C')) {
                var $btn = $('#btnMarkComplete');
                if ($btn.length && !$btn.prop('disabled')) {
                    $btn.click();
                }
            } else if (e.altKey && (e.key === 'f' || e.key === 'F')) {
                $('#btnToggleTheater').click();
            }
        });
    };

    /* ==========================================================================
       4. Quiz Assessment Engine (Palette, Timer, Flagging)
       ========================================================================== */
    LMS.initQuiz = function (totalQuestions, totalSeconds) {
        var remainingSeconds = totalSeconds;
        var isSubmitting = false;

        // Render palette buttons
        var $palette = $('#quizQuestionPalette');
        if ($palette.length) {
            $palette.empty();
            for (var i = 0; i < totalQuestions; i++) {
                var btn = $('<button type="button" class="quiz-palette-btn" data-target-q="' + i + '">' + (i + 1) + '</button>');
                $palette.append(btn);
            }
        }

        function updateQuizState() {
            var answeredCount = 0;
            for (var i = 0; i < totalQuestions; i++) {
                var isChecked = $('input[name="Answers[' + i + '].SelectedOptionId"]:checked').length > 0;
                var $palBtn = $('.quiz-palette-btn[data-target-q="' + i + '"]');
                var isFlagged = $('#flag_q_' + i).is(':checked');

                if (isChecked) {
                    answeredCount++;
                    $palBtn.addClass('palette-answered');
                } else {
                    $palBtn.removeClass('palette-answered');
                }

                if (isFlagged) {
                    $palBtn.addClass('palette-flagged');
                } else {
                    $palBtn.removeClass('palette-flagged');
                }
            }

            $('#answeredCounter').text(answeredCount + ' / ' + totalQuestions + ' Answered');
            var pct = (answeredCount / totalQuestions) * 100;
            $('#quizAnsweredProgressBar').css('width', pct + '%');
        }

        // Jump to question when clicking palette button
        $(document).on('click', '.quiz-palette-btn', function () {
            var qIdx = $(this).data('target-q');
            var $targetCard = $('.question-card[data-qindex="' + qIdx + '"]');
            if ($targetCard.length) {
                $('html, body').animate({
                    scrollTop: $targetCard.offset().top - 90
                }, 300);
                $('.quiz-palette-btn').removeClass('palette-active');
                $(this).addClass('palette-active');
            }
        });

        // Radio option container click & change handlers
        $(document).on('click', '.quiz-option-container', function (e) {
            if ($(e.target).is('.quiz-option-radio') || $(e.target).is('label')) {
                return; // Let native radio/label handle it
            }
            var $radio = $(this).find('.quiz-option-radio');
            if ($radio.length && !$radio.prop('checked')) {
                $radio.prop('checked', true).trigger('change');
            }
        });

        $('.quiz-option-radio').on('change', function () {
            var $card = $(this).closest('.question-card');
            $card.find('.quiz-option-container').removeClass('selected');
            $(this).closest('.quiz-option-container').addClass('selected');
            updateQuizState();
        });

        // Flag toggle handler
        $('.flag-toggle-checkbox').on('change', function () {
            updateQuizState();
            var qIdx = $(this).data('qindex');
            if ($(this).is(':checked')) {
                LMS.toast('Question ' + (qIdx + 1) + ' flagged for review', 'warning', 2000);
            }
        });

        // Timer
        var $timeDisplay = $('#timeRemainingDisplay');
        var $timerBar = $('#quizTimerBar');
        var $timerBadge = $('#timerBadge');

        function formatTime(totalSecs) {
            var m = Math.floor(totalSecs / 60);
            var s = totalSecs % 60;
            return (m < 10 ? '0' : '') + m + ':' + (s < 10 ? '0' : '') + s;
        }

        // Format immediately
        if ($timeDisplay.length) {
            $timeDisplay.text(formatTime(remainingSeconds));
        }

        var timerInterval = setInterval(function () {
            if (remainingSeconds <= 0) {
                clearInterval(timerInterval);
                if (!isSubmitting) {
                    isSubmitting = true;
                    LMS.toast('Time has expired! Submitting answers...', 'danger', 4000);
                    $('#quizForm').submit();
                }
                return;
            }

            remainingSeconds--;
            $timeDisplay.text(formatTime(remainingSeconds));

            var pct = (remainingSeconds / totalSeconds) * 100;
            $timerBar.css('width', pct + '%');

            if (remainingSeconds <= 60) {
                $timerBadge.addClass('timer-pulse-danger');
                $timerBar.removeClass('bg-success bg-warning').addClass('bg-danger');
            } else if (remainingSeconds <= 300) {
                $timerBar.removeClass('bg-success').addClass('bg-warning');
            }
        }, 1000);

        $('#quizForm').on('submit', function () {
            isSubmitting = true;
            clearInterval(timerInterval);
        });

        updateQuizState();
    };

    /* ==========================================================================
       5. Client-Side Live Catalog Search & Instant Filter
       ========================================================================== */
    LMS.initCatalogFilter = function () {
        var $input = $('#catalogSearchInput');
        var $cards = $('.course-catalog-item');
        var $emptyMsg = $('#catalogNoMatchMessage');
        var $levelSelect = $('select[name="level"]');

        function filterItems() {
            var query = ($input.val() || '').toLowerCase().trim();
            var activeCategory = $('.category-chip-btn.active').data('cat-id') || '';
            var activeLevel = $('.level-chip-btn.active').data('level') || ($levelSelect.length ? $levelSelect.val() : '') || '';
            if (activeLevel === 'All') activeLevel = '';

            var visibleCount = 0;

            $cards.each(function () {
                var $card = $(this);
                var title = ($card.data('title') || '').toLowerCase();
                var catId = ($card.data('cat-id') || '').toString();
                var level = ($card.data('level') || '').toLowerCase();

                var matchesText = !query || title.indexOf(query) !== -1;
                var matchesCategory = !activeCategory || catId === activeCategory;
                var matchesLevel = !activeLevel || level === activeLevel.toLowerCase();

                if (matchesText && matchesCategory && matchesLevel) {
                    $card.fadeIn(150);
                    visibleCount++;
                } else {
                    $card.fadeOut(150);
                }
            });

            if (visibleCount === 0) {
                $emptyMsg.removeClass('d-none');
            } else {
                $emptyMsg.addClass('d-none');
            }
        }

        if ($input.length) {
            $input.on('input', filterItems);
        }

        $(document).on('click', '.category-chip-btn', function (e) {
            e.preventDefault();
            $('.category-chip-btn').removeClass('active btn-primary').addClass('btn-light border');
            $(this).removeClass('btn-light border').addClass('active btn-primary');
            filterItems();
        });

        $(document).on('click', '.level-chip-btn', function (e) {
            e.preventDefault();
            $('.level-chip-btn').removeClass('active btn-primary').addClass('btn-outline-secondary');
            $(this).removeClass('btn-outline-secondary').addClass('active btn-primary');
            var lvl = $(this).data('level') || '';
            $('#filterLevelInput').val(lvl);
            filterItems();
        });
    };

    /* ==========================================================================
       6. Certificate Utilities (Share to LinkedIn, Copy Link)
       ========================================================================== */
    LMS.initCertificate = function () {
        $('#btnCopyCertUrl').on('click', function () {
            var url = window.location.href;
            if (navigator.clipboard) {
                navigator.clipboard.writeText(url).then(function () {
                    LMS.toast('Certificate verification URL copied to clipboard!', 'success');
                });
            } else {
                var temp = $('<input>');
                $('body').append(temp);
                temp.val(url).select();
                document.execCommand('copy');
                temp.remove();
                LMS.toast('Certificate verification URL copied to clipboard!', 'success');
            }
        });

        $('#btnShareLinkedIn').on('click', function () {
            var url = encodeURIComponent(window.location.href);
            var title = encodeURIComponent(document.title);
            var shareUrl = 'https://www.linkedin.com/sharing/share-offsite/?url=' + url;
            window.open(shareUrl, '_blank', 'width=600,height=500');
        });
    };

    /* ==========================================================================
       7. Global Page Preloader & Route Navigation Progress Engine
       ========================================================================== */
    LMS.initGlobalLoader = function () {
        var $preloader = $('#globalPreloader');
        var $progressBar = $('#routeProgressBar');
        var $loadingPill = $('#routeLoadingPill');
        var progressTimer;

        // Dismiss preloader once the DOM and assets are fully loaded
        function dismissPreloader() {
            if ($preloader.length && !$preloader.hasClass('fade-out')) {
                $preloader.addClass('fade-out');
                setTimeout(function () {
                    $preloader.remove();
                }, 400);
            }
        }

        // Window load event
        $(window).on('load', function () {
            dismissPreloader();
        });

        // Safety fallback so users are never blocked even on slow assets
        setTimeout(dismissPreloader, 650);

        // Route Progress Bar Controller
        LMS.startLoading = function () {
            if (!$progressBar.length) return;
            clearInterval(progressTimer);
            $progressBar.removeClass('running').css({ width: '0%', opacity: 1 }).addClass('running');
            $loadingPill.css('display', 'flex');

            var current = 15;
            $progressBar.css('width', current + '%');

            progressTimer = setInterval(function () {
                if (current < 85) {
                    current += Math.random() * 12 + 5;
                    $progressBar.css('width', current + '%');
                }
            }, 250);
        };

        LMS.finishLoading = function () {
            if (!$progressBar.length) return;
            clearInterval(progressTimer);
            $progressBar.css('width', '100%');
            setTimeout(function () {
                $progressBar.css('opacity', '0');
                $loadingPill.fadeOut(200);
                setTimeout(function () {
                    $progressBar.css('width', '0%').removeClass('running');
                }, 300);
            }, 200);
        };

        // Intercept standard navigation link clicks to show responsive loading progress
        $(document).on('click', 'a[href]', function (e) {
            var href = $(this).attr('href');
            var target = $(this).attr('target');
            var isDownload = $(this).attr('download') !== undefined || $(this).hasClass('no-loader');

            if (!href || href === '#' || href.indexOf('javascript:') === 0 || href.indexOf('#') === 0 || target === '_blank' || isDownload || e.ctrlKey || e.metaKey || e.shiftKey) {
                return;
            }

            // Internal application route transition
            LMS.startLoading();
        });

        // Intercept form submissions
        $(document).on('submit', 'form:not([data-ajax="true"]):not(.no-loader)', function () {
            if ($(this).valid && !$(this).valid()) return;
            LMS.startLoading();
        });

        // Trigger on beforeunload
        window.addEventListener('beforeunload', function () {
            LMS.startLoading();
        });

        // Global jQuery AJAX Listeners
        $(document).ajaxStart(function () {
            LMS.startLoading();
        });
        $(document).ajaxStop(function () {
            LMS.finishLoading();
        });
    };

    /* ==========================================================================
       8. UI Initializers (Tooltips, Password Toggles, Preloader)
       ========================================================================== */
    $(function () {
        // Initialize Global Loading System
        LMS.initGlobalLoader();

        // Init Bootstrap tooltips if available
        if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
            var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
            tooltipTriggerList.map(function (tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl);
            });
        }

        // Password visibility toggles
        $('.toggle-password-btn').on('click', function () {
            var $input = $(this).closest('.input-group').find('input');
            var type = $input.attr('type') === 'password' ? 'text' : 'password';
            $input.attr('type', type);
            $(this).find('i').toggleClass('bi-eye bi-eye-slash');
        });
    });

})(window, document, jQuery);
