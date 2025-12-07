// Fix for icons when opening HTML files directly (file:// protocol)
// This script converts img tags to inline SVG to avoid CORS issues

(function() {
    'use strict';
    
    // Wait for DOM to be ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', fixIcons);
    } else {
        fixIcons();
    }
    
    function fixIcons() {
        // Get all nav icon images
        const navIcons = document.querySelectorAll('.page__nav__link__icon');
        
        navIcons.forEach(img => {
            // Skip if already an SVG element
            if (img.tagName === 'SVG') return;
            
            const src = img.getAttribute('src');
            if (!src || !src.endsWith('.svg')) return;
            
            // Fetch and replace with inline SVG
            fetch(src)
                .then(response => response.text())
                .then(svgContent => {
                    // Create a temporary div to parse SVG
                    const temp = document.createElement('div');
                    temp.innerHTML = svgContent.trim();
                    const svg = temp.querySelector('svg');
                    
                    if (svg) {
                        // Copy classes from img to svg
                        svg.classList.add('page__nav__link__icon');
                        
                        // Replace img with svg
                        img.parentNode.replaceChild(svg, img);
                    }
                })
                .catch(err => console.error('Error loading icon:', src, err));
        });
    }
})();

