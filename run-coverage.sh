#!/bin/bash

# YouTubeCLI Code Coverage Runner
# This script runs the test suite with code coverage collection and generates
# a comprehensive HTML report showing line and branch coverage.

set -e

# Default values
OUTPUT_DIR="./coverage-output"
OPEN_REPORT=false

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
WHITE='\033[1;37m'
NC='\033[0m' # No Color

# Function to print colored output
print_color() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Function to show usage
show_usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  -o, --output DIR    Output directory for coverage reports (default: ./coverage-output)"
    echo "  -h, --help          Show this help message"
    echo "  --open              Open the HTML report in browser after generation"
    echo ""
    echo "Examples:"
    echo "  $0"
    echo "  $0 --output ./my-coverage"
    echo "  $0 --open"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -o|--output)
            OUTPUT_DIR="$2"
            shift 2
            ;;
        --open)
            OPEN_REPORT=true
            shift
            ;;
        -h|--help)
            show_usage
            exit 0
            ;;
        *)
            print_color $RED "Unknown option: $1"
            show_usage
            exit 1
            ;;
    esac
done

print_color $GREEN "🔍 Running Code Coverage Analysis..."

# Clean previous coverage data
if [ -d "$OUTPUT_DIR" ]; then
    print_color $YELLOW "🧹 Cleaning previous coverage data..."
    rm -rf "$OUTPUT_DIR"
fi

# Create output directory
mkdir -p "$OUTPUT_DIR"

# Run tests with coverage
print_color $CYAN "🧪 Running tests with coverage collection..."
dotnet test tests/YouTubeCLI.Tests/YouTubeCLI.Tests.csproj \
    --configuration Release \
    --verbosity normal \
    --collect:"XPlat Code Coverage" \
    --results-directory "$OUTPUT_DIR/test-results" \
    --logger "trx;LogFileName=test-results.trx" \
    --settings coverlet.runsettings

if [ $? -ne 0 ]; then
    print_color $RED "❌ Tests failed!"
    exit 1
fi

# Install ReportGenerator if not already installed
print_color $CYAN "📦 Installing ReportGenerator..."
dotnet tool install -g dotnet-reportgenerator-globaltool --version 5.2.0 > /dev/null 2>&1

# Find coverage files
COVERAGE_FILES=$(find "$OUTPUT_DIR/test-results" -name "coverage.cobertura.xml" 2>/dev/null || true)

if [ -z "$COVERAGE_FILES" ]; then
    print_color $RED "❌ No coverage files found!"
    exit 1
fi

COVERAGE_COUNT=$(echo "$COVERAGE_FILES" | wc -l)
print_color $GREEN "📊 Found $COVERAGE_COUNT coverage file(s)"

# Generate HTML report
print_color $CYAN "📈 Generating coverage report..."
COVERAGE_FILES_STRING=$(echo "$COVERAGE_FILES" | tr '\n' ';')

reportgenerator \
    -reports:"$COVERAGE_FILES_STRING" \
    -targetdir:"$OUTPUT_DIR/html-report" \
    -reporttypes:"Html;Cobertura;JsonSummary" \
    -assemblyfilters:"-*.Tests*" \
    -classfilters:"-*.Tests*" \
    -filefilters:"-*.Tests*" \
    -verbosity:Info

# Display coverage summary
SUMMARY_FILE="$OUTPUT_DIR/html-report/Summary.json"
if [ -f "$SUMMARY_FILE" ]; then
    LINE_COVERAGE=$(cat "$SUMMARY_FILE" | jq -r '.linecoverage' 2>/dev/null || echo "0")
    BRANCH_COVERAGE=$(cat "$SUMMARY_FILE" | jq -r '.branchcoverage' 2>/dev/null || echo "0")
    TOTAL_LINES=$(cat "$SUMMARY_FILE" | jq -r '.totallines' 2>/dev/null || echo "0")
    COVERED_LINES=$(cat "$SUMMARY_FILE" | jq -r '.coveredlines' 2>/dev/null || echo "0")

    echo ""
    print_color $GREEN "📊 Coverage Summary:"
    print_color $GREEN "==================="

    # Line coverage with color
    if (( $(echo "$LINE_COVERAGE >= 80" | bc -l) )); then
        print_color $GREEN "Line Coverage:   $(printf "%.2f" $LINE_COVERAGE)%"
    else
        print_color $YELLOW "Line Coverage:   $(printf "%.2f" $LINE_COVERAGE)%"
    fi

    # Branch coverage with color
    if (( $(echo "$BRANCH_COVERAGE >= 70" | bc -l) )); then
        print_color $GREEN "Branch Coverage: $(printf "%.2f" $BRANCH_COVERAGE)%"
    else
        print_color $YELLOW "Branch Coverage: $(printf "%.2f" $BRANCH_COVERAGE)%"
    fi

    print_color $WHITE "Total Lines:     $TOTAL_LINES"
    print_color $WHITE "Covered Lines:   $COVERED_LINES"

    # Coverage status
    echo ""
    print_color $GREEN "🎯 Coverage Status:"

    if (( $(echo "$LINE_COVERAGE >= 80" | bc -l) )); then
        print_color $WHITE "Line Coverage:   ✅ Target met"
    else
        print_color $WHITE "Line Coverage:   ⚠️  Below target (80%)"
    fi

    if (( $(echo "$BRANCH_COVERAGE >= 70" | bc -l) )); then
        print_color $WHITE "Branch Coverage: ✅ Target met"
    else
        print_color $WHITE "Branch Coverage: ⚠️  Below target (70%)"
    fi
fi

echo ""
print_color $GREEN "📁 Coverage Reports Generated:"
print_color $CYAN "HTML Report: $OUTPUT_DIR/html-report/index.html"
print_color $CYAN "Raw Data:    $OUTPUT_DIR/test-results/"

# Open report if requested
if [ "$OPEN_REPORT" = true ]; then
    HTML_REPORT="$OUTPUT_DIR/html-report/index.html"
    if [ -f "$HTML_REPORT" ]; then
        print_color $CYAN "🌐 Opening coverage report in browser..."
        if command -v xdg-open > /dev/null; then
            xdg-open "$HTML_REPORT"
        elif command -v open > /dev/null; then
            open "$HTML_REPORT"
        else
            print_color $YELLOW "⚠️  Could not open browser automatically. Please open: $HTML_REPORT"
        fi
    fi
fi

print_color $GREEN ""
print_color $GREEN "✅ Coverage analysis complete!"
