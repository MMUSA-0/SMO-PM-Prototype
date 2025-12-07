#!/usr/bin/env python3
"""
Script to add development indicators to all existing pages
"""

import os
import re

# Configuration for pages and their development status
page_status = {
    'milestone-tracking.html': {
        'status': 'partial',
        'progress': 60,
        'features': ['Gantt Chart', 'Excel Export', 'Detailed Reports']
    },
    'achievements.html': {
        'status': 'partial',
        'progress': 70,
        'features': ['File Uploads', 'Achievement Verification']
    },
    'performance-initiatives.html': {
        'status': 'partial',
        'progress': 75,
        'features': ['Documents Section', 'KPI Linking']
    },
    'risk-escalation.html': {
        'status': 'partial',
        'progress': 50,
        'features': ['Auto Escalation', 'Risk Dashboard', 'Smart Alerts']
    },
    'audit-logs.html': {
        'status': 'partial',
        'progress': 40,
        'features': ['Advanced Search', 'PDF Export', 'Type Filtering']
    },
    'profile.html': {
        'status': 'partial',
        'progress': 65,
        'features': ['Avatar Upload', 'Notification Settings']
    },
    'settings.html': {
        'status': 'partial',
        'progress': 55,
        'features': ['Language Settings', 'Backup', 'API Management']
    },
    'indicators.html': {
        'status': 'partial',
        'progress': 65,
        'features': ['Import Indicators', 'Excel Export']
    },
    'initiative-details.html': {
        'status': 'partial',
        'progress': 70,
        'features': ['Attachments', 'Change Log', 'Comments']
    },
    'performance-thresholds.html': {
        'status': 'partial',
        'progress': 45,
        'features': ['Custom Thresholds', 'Auto Alerts']
    },
    'performance-requests.html': {
        'status': 'partial',
        'progress': 60,
        'features': ['Request Tracking', 'Multi-approvals']
    },
    'data-sync-status.html': {
        'status': 'partial',
        'progress': 50,
        'features': ['Auto Sync', 'Detailed Logs', 'Auto Retry']
    }
}

def add_dev_indicators(file_path):
    """Add development indicators to HTML file"""
    
    file_name = os.path.basename(file_path)
    
    # Skip if not in our list
    if file_name not in page_status:
        print(f"✓ {file_name} - No development indicators needed")
        return False
    
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            html = f.read()
        
        # Check if already has dev indicators
        if 'dev-indicators.css' in html and 'dev-indicators.js' in html:
            print(f"✓ {file_name} - Already has development indicators")
            return False
        
        # Add CSS link before </head>
        if 'dev-indicators.css' not in html:
            css_link = '    <link href="assets/css/dev-indicators.css" rel="stylesheet">\n'
            html = html.replace('</head>', f'{css_link}</head>')
        
        # Add JS script before </body>
        if 'dev-indicators.js' not in html:
            js_script = '    <script src="assets/js/dev-indicators.js"></script>\n'
            html = html.replace('</body>', f'{js_script}</body>')
        
        # Add data attributes to body tag
        page_info = page_status[file_name]
        body_pattern = r'<body([^>]*)>'
        body_match = re.search(body_pattern, html)
        
        if body_match:
            existing_attrs = body_match.group(1)
            if 'data-dev-status' not in existing_attrs:
                new_body = f'<body{existing_attrs} data-dev-status="{page_info["status"]}" data-dev-progress="{page_info["progress"]}">'
                html = re.sub(body_pattern, new_body, html, count=1)
        
        # Save the file
        with open(file_path, 'w', encoding='utf-8') as f:
            f.write(html)
        
        print(f"✅ {file_name} - Added development indicators ({page_info['progress']}% complete)")
        return True
        
    except Exception as e:
        print(f"❌ Error processing {file_name}: {e}")
        return False

def main():
    """Process all HTML files"""
    
    print("Adding development indicators to pages...\n")
    
    # Get current directory
    current_dir = os.path.dirname(os.path.abspath(__file__))
    
    # Process all HTML files
    updated_count = 0
    for file in os.listdir(current_dir):
        if file.endswith('.html') and not file.startswith('template') and file != 'START-HERE.html':
            file_path = os.path.join(current_dir, file)
            if add_dev_indicators(file_path):
                updated_count += 1
    
    print(f"\n✅ Updated {updated_count} pages with development indicators")
    
    print("\nPages with development status:")
    for page, info in page_status.items():
        print(f"  - {page}: {info['progress']}% complete")

if __name__ == "__main__":
    main()

