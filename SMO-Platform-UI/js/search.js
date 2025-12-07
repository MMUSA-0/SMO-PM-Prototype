/**
 * Search Functionality for SMO Platform
 * Provides real-time search capabilities across the platform
 */

(function() {
    'use strict';

    // Search configuration
    const SearchConfig = {
        minSearchLength: 2,
        debounceDelay: 300,
        maxResults: 50,
        highlightClass: 'search-highlight'
    };

    // Search state
    let searchTimeout = null;
    let currentSearchTerm = '';
    let isSearching = false;

    /**
     * Initialize search functionality on page load
     */
    document.addEventListener('DOMContentLoaded', function() {
        initializeSearch();
        setupKeyboardShortcuts();
    });

    /**
     * Initialize all search inputs on the page
     */
    function initializeSearch() {
        const searchInputs = document.querySelectorAll('.search-menu input[type="search"]');
        
        searchInputs.forEach(input => {
            // Add search event handlers
            input.addEventListener('input', handleSearchInput);
            input.addEventListener('keypress', handleSearchKeypress);
            input.addEventListener('focus', handleSearchFocus);
            input.addEventListener('blur', handleSearchBlur);
            
            // Add clear button
            addClearButton(input);
        });
        
        // Initialize search toggle buttons
        const searchToggles = document.querySelectorAll('.search-toggle-btn');
        searchToggles.forEach(btn => {
            btn.addEventListener('click', toggleSearch);
        });
    }

    /**
     * Handle search input changes with debouncing
     */
    function handleSearchInput(event) {
        const query = event.target.value.trim();
        
        // Clear previous timeout
        if (searchTimeout) {
            clearTimeout(searchTimeout);
        }
        
        // Show/hide clear button
        const clearBtn = event.target.parentElement.querySelector('.search-clear-btn');
        if (clearBtn) {
            clearBtn.style.display = query.length > 0 ? 'block' : 'none';
        }
        
        // Debounce search
        searchTimeout = setTimeout(() => {
            if (query.length >= SearchConfig.minSearchLength) {
                performSearch(query);
            } else if (query.length === 0) {
                clearSearchResults();
            }
        }, SearchConfig.debounceDelay);
    }

    /**
     * Handle Enter key in search input
     */
    function handleSearchKeypress(event) {
        if (event.key === 'Enter') {
            event.preventDefault();
            const query = event.target.value.trim();
            if (query.length >= SearchConfig.minSearchLength) {
                performSearch(query, true);
            }
        }
    }

    /**
     * Handle search input focus
     */
    function handleSearchFocus(event) {
        event.target.parentElement.classList.add('search-focused');
        
        // Show previous results if available
        if (currentSearchTerm && !isSearching) {
            showSearchResults();
        }
    }

    /**
     * Handle search input blur
     */
    function handleSearchBlur(event) {
        event.target.parentElement.classList.remove('search-focused');
        
        // Hide results after a delay (to allow clicking on results)
        setTimeout(() => {
            hideSearchResults();
        }, 200);
    }

    /**
     * Add clear button to search input
     */
    function addClearButton(input) {
        const wrapper = input.parentElement;
        
        const clearBtn = document.createElement('button');
        clearBtn.className = 'search-clear-btn';
        clearBtn.innerHTML = '<i class="bi bi-x-circle"></i>';
        clearBtn.style.cssText = `
            position: absolute;
            left: 10px;
            top: 50%;
            transform: translateY(-50%);
            border: none;
            background: none;
            color: #999;
            cursor: pointer;
            display: none;
            z-index: 10;
        `;
        
        clearBtn.addEventListener('click', function() {
            input.value = '';
            input.focus();
            clearSearchResults();
            this.style.display = 'none';
        });
        
        wrapper.style.position = 'relative';
        wrapper.appendChild(clearBtn);
    }

    /**
     * Toggle search visibility
     */
    function toggleSearch(event) {
        event.preventDefault();
        const searchMenu = event.target.closest('.search-menu');
        const input = searchMenu.querySelector('input[type="search"]');
        
        searchMenu.classList.toggle('search-open');
        
        if (searchMenu.classList.contains('search-open')) {
            input.focus();
        }
    }

    /**
     * Perform search
     */
    function performSearch(query, immediate = false) {
        if (isSearching && !immediate) {
            return;
        }
        
        currentSearchTerm = query;
        isSearching = true;
        
        // Show loading state
        showSearchLoading();
        
        // Check if API service is available
        if (window.apiService && window.apiService.search) {
            // Use API search
            window.apiService.search(query)
                .then(results => {
                    displaySearchResults(results);
                })
                .catch(error => {
                    console.error('Search error:', error);
                    performClientSearch(query);
                })
                .finally(() => {
                    isSearching = false;
                });
        } else {
            // Fallback to client-side search
            performClientSearch(query);
            isSearching = false;
        }
    }

    /**
     * Perform client-side search
     */
    function performClientSearch(query) {
        const results = [];
        const lowerQuery = query.toLowerCase();
        
        // Search in different page elements
        const searchableElements = [
            { selector: '.card', type: 'بطاقة' },
            { selector: '.table tbody tr', type: 'جدول' },
            { selector: '.list-group-item', type: 'قائمة' },
            { selector: 'h1, h2, h3, h4, h5, h6', type: 'عنوان' }
        ];
        
        searchableElements.forEach(({ selector, type }) => {
            const elements = document.querySelectorAll(selector);
            elements.forEach(element => {
                const text = element.textContent || '';
                const lowerText = text.toLowerCase();
                
                if (lowerText.includes(lowerQuery)) {
                    const excerpt = extractExcerpt(text, query);
                    results.push({
                        title: extractTitle(element),
                        excerpt: excerpt,
                        type: type,
                        element: element
                    });
                }
            });
        });
        
        // Limit results
        const limitedResults = results.slice(0, SearchConfig.maxResults);
        displaySearchResults(limitedResults);
    }

    /**
     * Extract title from element
     */
    function extractTitle(element) {
        // Try to find a heading within the element
        const heading = element.querySelector('h1, h2, h3, h4, h5, h6');
        if (heading) {
            return heading.textContent.trim();
        }
        
        // Use first line of text
        const text = element.textContent || '';
        const lines = text.trim().split('\n');
        return lines[0].substring(0, 100).trim();
    }

    /**
     * Extract excerpt with context around search term
     */
    function extractExcerpt(text, query) {
        const lowerText = text.toLowerCase();
        const lowerQuery = query.toLowerCase();
        const index = lowerText.indexOf(lowerQuery);
        
        if (index === -1) {
            return text.substring(0, 150) + '...';
        }
        
        const start = Math.max(0, index - 50);
        const end = Math.min(text.length, index + query.length + 50);
        
        let excerpt = text.substring(start, end);
        
        // Add ellipsis if needed
        if (start > 0) excerpt = '...' + excerpt;
        if (end < text.length) excerpt = excerpt + '...';
        
        // Highlight the search term
        const regex = new RegExp(`(${escapeRegex(query)})`, 'gi');
        excerpt = excerpt.replace(regex, '<mark>$1</mark>');
        
        return excerpt;
    }

    /**
     * Display search results
     */
    function displaySearchResults(results) {
        let resultsContainer = document.getElementById('searchResults');
        
        if (!resultsContainer) {
            resultsContainer = createSearchResultsContainer();
        }
        
        if (results.length === 0) {
            resultsContainer.innerHTML = `
                <div class="search-no-results">
                    <i class="bi bi-search"></i>
                    <p>لم يتم العثور على نتائج لـ "${currentSearchTerm}"</p>
                    <small>جرب استخدام كلمات مختلفة أو أكثر عمومية</small>
                </div>
            `;
        } else {
            const resultsHTML = results.map(result => `
                <div class="search-result-item" data-element-id="${result.element ? result.element.id : ''}">
                    <div class="search-result-type">${result.type}</div>
                    <div class="search-result-title">${result.title}</div>
                    <div class="search-result-excerpt">${result.excerpt}</div>
                </div>
            `).join('');
            
            resultsContainer.innerHTML = `
                <div class="search-results-header">
                    <span>عدد النتائج: ${results.length}</span>
                    <button class="btn btn-sm btn-outline-secondary" onclick="clearSearchResults()">
                        <i class="bi bi-x"></i> إغلاق
                    </button>
                </div>
                <div class="search-results-list">
                    ${resultsHTML}
                </div>
            `;
            
            // Add click handlers to results
            resultsContainer.querySelectorAll('.search-result-item').forEach((item, index) => {
                item.addEventListener('click', () => {
                    scrollToResult(results[index]);
                });
            });
        }
        
        showSearchResults();
    }

    /**
     * Create search results container
     */
    function createSearchResultsContainer() {
        const container = document.createElement('div');
        container.id = 'searchResults';
        container.className = 'search-results-container';
        container.style.cssText = `
            position: fixed;
            top: 70px;
            left: 50%;
            transform: translateX(-50%);
            width: 90%;
            max-width: 600px;
            max-height: 70vh;
            background: white;
            border: 1px solid #ddd;
            border-radius: 8px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            z-index: 9999;
            display: none;
            overflow: auto;
        `;
        
        document.body.appendChild(container);
        
        // Add styles if not already added
        if (!document.getElementById('searchStyles')) {
            const styles = document.createElement('style');
            styles.id = 'searchStyles';
            styles.innerHTML = `
                .search-results-header {
                    padding: 10px 15px;
                    background: #f8f9fa;
                    border-bottom: 1px solid #ddd;
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    position: sticky;
                    top: 0;
                    z-index: 10;
                }
                
                .search-results-list {
                    padding: 10px;
                }
                
                .search-result-item {
                    padding: 10px;
                    border-bottom: 1px solid #eee;
                    cursor: pointer;
                    transition: background-color 0.2s;
                }
                
                .search-result-item:hover {
                    background-color: #f8f9fa;
                }
                
                .search-result-type {
                    font-size: 0.8em;
                    color: #6c757d;
                    margin-bottom: 3px;
                }
                
                .search-result-title {
                    font-weight: bold;
                    margin-bottom: 5px;
                    color: #333;
                }
                
                .search-result-excerpt {
                    font-size: 0.9em;
                    color: #666;
                }
                
                .search-result-excerpt mark {
                    background-color: #ffeb3b;
                    padding: 0 2px;
                    font-weight: bold;
                }
                
                .search-no-results {
                    text-align: center;
                    padding: 50px 20px;
                    color: #999;
                }
                
                .search-no-results i {
                    font-size: 3em;
                    margin-bottom: 15px;
                    display: block;
                }
                
                .search-highlight {
                    background-color: #ffeb3b !important;
                    padding: 2px 4px;
                    border-radius: 3px;
                    animation: highlight-fade 2s ease-out;
                }
                
                @keyframes highlight-fade {
                    0% {
                        background-color: #ffeb3b;
                    }
                    100% {
                        background-color: transparent;
                    }
                }
                
                .search-loading {
                    text-align: center;
                    padding: 50px;
                }
                
                .search-loading .spinner-border {
                    width: 3rem;
                    height: 3rem;
                }
            `;
            document.head.appendChild(styles);
        }
        
        return container;
    }

    /**
     * Show search loading state
     */
    function showSearchLoading() {
        let container = document.getElementById('searchResults');
        
        if (!container) {
            container = createSearchResultsContainer();
        }
        
        container.innerHTML = `
            <div class="search-loading">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">جاري البحث...</span>
                </div>
                <p class="mt-3">جاري البحث عن "${currentSearchTerm}"...</p>
            </div>
        `;
        
        showSearchResults();
    }

    /**
     * Show search results container
     */
    function showSearchResults() {
        const container = document.getElementById('searchResults');
        if (container) {
            container.style.display = 'block';
        }
    }

    /**
     * Hide search results container
     */
    function hideSearchResults() {
        const container = document.getElementById('searchResults');
        if (container) {
            container.style.display = 'none';
        }
    }

    /**
     * Clear search results
     */
    window.clearSearchResults = function() {
        currentSearchTerm = '';
        hideSearchResults();
        removeHighlights();
        
        // Clear search inputs
        const searchInputs = document.querySelectorAll('.search-menu input[type="search"]');
        searchInputs.forEach(input => {
            input.value = '';
            const clearBtn = input.parentElement.querySelector('.search-clear-btn');
            if (clearBtn) {
                clearBtn.style.display = 'none';
            }
        });
    };

    /**
     * Scroll to search result
     */
    function scrollToResult(result) {
        if (result.element) {
            // Remove previous highlights
            removeHighlights();
            
            // Highlight the element
            result.element.classList.add(SearchConfig.highlightClass);
            
            // Scroll to element
            result.element.scrollIntoView({
                behavior: 'smooth',
                block: 'center'
            });
            
            // Remove highlight after animation
            setTimeout(() => {
                result.element.classList.remove(SearchConfig.highlightClass);
            }, 2000);
            
            // Hide search results
            hideSearchResults();
        }
    }

    /**
     * Remove all highlights
     */
    function removeHighlights() {
        document.querySelectorAll('.' + SearchConfig.highlightClass).forEach(el => {
            el.classList.remove(SearchConfig.highlightClass);
        });
    }

    /**
     * Setup keyboard shortcuts
     */
    function setupKeyboardShortcuts() {
        document.addEventListener('keydown', function(event) {
            // Ctrl+K or Cmd+K to focus search
            if ((event.ctrlKey || event.metaKey) && event.key === 'k') {
                event.preventDefault();
                const searchInput = document.querySelector('.search-menu input[type="search"]');
                if (searchInput) {
                    const searchMenu = searchInput.closest('.search-menu');
                    searchMenu.classList.add('search-open');
                    searchInput.focus();
                }
            }
            
            // Escape to close search
            if (event.key === 'Escape') {
                clearSearchResults();
            }
        });
    }

    /**
     * Escape regex special characters
     */
    function escapeRegex(string) {
        return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    }

})();