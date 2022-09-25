// @ts-check

/** @type {import('@docusaurus/plugin-content-docs').SidebarsConfig} */
const sidebars = {
    docs: [
        'introduction',
        {
            type: 'category',
            label: 'Unit testing',
            link: {
                type: 'generated-index',
            },
            collapsed: true,
            items: [
                {
                    type: 'category',
                    label: 'Getting started',
                    link: {
                        type: 'generated-index',
                    },
                    collapsed: true,
                    items: [
                        'unit-testing/getting-started/what-is-unit-testing',
                        'unit-testing/getting-started/the-calculator',
                        'unit-testing/getting-started/creating-test-project',
                        'unit-testing/getting-started/your-first-test',
                        'unit-testing/getting-started/lets-improve',
                        'unit-testing/getting-started/arrange-act-assert',
                        'unit-testing/getting-started/calculator-exercise',
                        'unit-testing/getting-started/multiple-test-params',
                        'unit-testing/getting-started/solution-structure'
                    ],
                },
                {
                    type: 'category',
                    label: 'Deep dive',
                    link: {
                        type: 'generated-index',
                    },
                    collapsed: true,
                    items: [
                        'unit-testing/deep-dive/api-intro',
                        'unit-testing/deep-dive/quote-service',
                        'unit-testing/deep-dive/attempting-test',
                        'unit-testing/deep-dive/mocking-intro',
                        'unit-testing/deep-dive/asserting-exceptions',
                        'unit-testing/deep-dive/verify-calls',
                        'unit-testing/deep-dive/exercise-quote',
                        'unit-testing/deep-dive/dealing-with-time',
                        'unit-testing/deep-dive/testing-internals',
                        'unit-testing/deep-dive/code-coverage'
                    ]
                },
                {
                    type: 'category',
                    label: 'Test driven development',
                    link: {
                        type: 'generated-index',
                    },
                    collapsed: true,
                    items: [
                        'unit-testing/tdd/tdd-intro',
                        'unit-testing/tdd/an-example'
                    ]
                }
            ],
        },
        {
            type: 'category',
            label: 'Mutation testing',
            link: {
                type: 'generated-index',
            },
            collapsed: true,
            items: [
                'mutation-testing/cc-criticism',
                'mutation-testing/mutation-testing',
                'mutation-testing/hands-on',
                'mutation-testing/exercise',
            ]
        },
        {
            type: 'category',
            label: 'Integration testing',
            link: {
                type: 'generated-index',
            },
            collapsed: true,
            items: [
                'integration-testing/introduction',
                {
                    type: 'category',
                    label: 'Testing an API',
                    link: {
                        type: 'generated-index',
                    },
                    collapsed: true,
                    items: [
                        'integration-testing/api/the-api',
                        'integration-testing/api/first-integration-test',
                        'integration-testing/api/leftover-data',
                        'integration-testing/api/using-waf',
                        'integration-testing/api/write-the-tests',
                        'integration-testing/api/our-own-waf',
                        'integration-testing/api/databases-on-demand',
                        'integration-testing/api/dealing-with-apis',
                        'integration-testing/api/realistic-fake-data',
                    ]
                },
                {
                    type: 'category',
                    label: 'Testing a Web App',
                    link: {
                        type: 'generated-index',
                    },
                    collapsed: true,
                    items: [
                        'integration-testing/ui/the-webapp',
                        'integration-testing/ui/same-approach',
                        'integration-testing/ui/running-for-testing',
                        'integration-testing/ui/test-setup',
                        'integration-testing/ui/dealing-with-docker-compose',
                        'integration-testing/ui/browser-testing',
                        'integration-testing/ui/first-integration-test',
                        'integration-testing/ui/more-tests',
                    ]
                }
            ]
        },
        {
            type: 'category',
            label: 'Performance testing',
            link: {
                type: 'generated-index',
            },
            collapsed: true,
            items: [
                'performance-testing/intro-to-perf-testing',
                'performance-testing/smoke-testing',
                'performance-testing/load-testing',
                'performance-testing/stress-testing',
                'performance-testing/soak-testing'
            ]
        }
    ]
};

module.exports = sidebars;
