---
description: Let's see how we can implement soak tests with k6
---

# Soak testing

Soak testing is all about reliability. It takes those relatively small timespans of minutes per stage and it turns them into
hours. Soak tests can run from 1 to 24 hours and the ultimate goal is to assess the long term reliability of the system.

Soak tests are used to:

- Detect memory leaks or random crashes
- Detect race-conditions that are hard to reproduce
- Detect issues such as socket exhaustion or database connectivity and storage issues
- Detect issues with log storage
- Detect issues with third party services

You should run soak tests when you think that your system suffers from a reliability issue. 
Even though soak tests are usually run against the real production or staging system, it is also very common
to run them locally with profiling tools hooked to your app to detect said issues.

## Implementing a soak test

Soak tests are even simpler than spike tests. They only have 3 stages:

1. Ramp up to normal load in 2 minutes
2. Sustain normal load for 1-24 hours
3. Ramp down to 0 users in 2 minutes

That's it!. The test method can have anything from single requests to batched one.

```js
import http from 'k6/http';
import { check, sleep} from 'k6';

export const options = {
    stages: [
        { duration: '2m', target: 400 },
        { duration: '1h56m', target: 400 },
        { duration: '2m', target: 0 }
    ],

    thresholds: {
        http_req_duration: ['p(99)<15'], 
    },
};

const BASE_URL = 'https://localhost:5001';

export default () => {
    const responses = http.batch([
        ['GET', `${BASE_URL}/customers/`, null],
        ['GET', `${BASE_URL}/customers/`, null],
        ['GET', `${BASE_URL}/customers/`, null],
        ['GET', `${BASE_URL}/customers/`, null],
    ]);

    responses.forEach(x => {
        const customers = x.json();
        check(customers, { 'retrieved customers': (obj) => obj.customers.length > 0 });
    })    
    sleep(1);
};
```

This is the graph that soak tests produce:

![](/img/performance/soak-test.jpg) 
