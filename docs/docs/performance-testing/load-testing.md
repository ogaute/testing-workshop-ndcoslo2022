---
description: Let's see how we can implement load tests with k6
---

# Load testing

Load testing's main purpose is to determine the system's ability to handle load both during normal operating condition 
and peak ones. To see how your system behaves during typical load and also during peak load.

If, for example, your app handles 20 concurrent users on average as normal load and 50 users during peak hour then you 
want to run a load test that tests for both numbers in the same execution.

## Load testing testing the Customers API

The main code from our Smoke Test, still applies here:

```js
import http from 'k6/http';
import { check, sleep} from 'k6';

export const options = {
    vus: 1,
    duration: '1m',

    thresholds: {
        http_req_duration: ['p(99)<15'], 
    },
};

const BASE_URL = 'https://localhost:5001';

export default () => {
    const customers = http.get(`${BASE_URL}/customers/`).json();
    check(customers, { 'retrieved customers': (obj) => obj.customers.length > 0 });
    sleep(1);
};
```

What will be changing is that we will no longer have a single stage and a static number of VUs.
We will remove `vus` and `duration` from the `options` object and replace them with a stages array.

```js
stages: [
    { duration: '5m', target: 50 },
    { duration: '10m', target: 50 },
    { duration: '5m', target: 0 },
]
```

The number of stages for our load test is determined by the number of items in the array.

This test reads as follows:

1. Start the test and for the first 5 minutes simulate ramp-up of traffic from 1 user to 50 over 5 minutes
2. For the next 10 minutes stay are 50 users
3. For the last 5 minutes of the test ramp-down to 0 users

## Running the simple load test

Running the load test produces the following results:

> _To save time run with 1 minute ramp-up, 2 minutes sustained and 1 minute ramp-down_

```commandline
          /\      |‾‾| /‾‾/   /‾‾/
     /\  /  \     |  |/  /   /  /
    /  \/    \    |     (   /   ‾‾\
   /          \   |  |\  \ |  (‾)  |
  / __________ \  |__| \__\ \_____/ .io

  execution: local
     script: ./load-test.js
     output: -

  scenarios: (100.00%) 1 scenario, 50 max VUs, 4m30s max duration (incl. graceful stop):
           * default: Up to 50 looping VUs for 4m0s over 3 stages (gracefulRampDown: 30s, gracefulStop: 30s)
           
running (4m00.4s), 00/50 VUs, 9001 complete and 0 interrupted iterations
default ✓ [======================================] 00/50 VUs  4m0s

     ✓ retrieved customers

     checks.........................: 100.00% ✓ 9001      ✗ 0
     data_received..................: 2.8 MB  12 kB/s
     data_sent......................: 418 kB  1.7 kB/s
     http_req_blocked...............: avg=38.33µs  min=0s       med=0s      max=15.99ms  p(90)=0s      p(95)=0s
     http_req_connecting............: avg=1.37µs   min=0s       med=0s      max=502.49µs p(90)=0s      p(95)=0s
   ✓ http_req_duration..............: avg=933.73µs min=301.39µs med=969µs   max=16.14ms  p(90)=1.24ms  p(95)=1.29ms
       { expected_response:true }...: avg=933.73µs min=301.39µs med=969µs   max=16.14ms  p(90)=1.24ms  p(95)=1.29ms
     http_req_failed................: 0.00%   ✓ 0         ✗ 9001
     http_req_receiving.............: avg=173.1µs  min=0s       med=121.3µs max=542.5µs  p(90)=457.4µs p(95)=499.9µs
     http_req_sending...............: avg=5.55µs   min=0s       med=0s      max=536.9µs  p(90)=0s      p(95)=0s
     http_req_tls_handshaking.......: avg=35.64µs  min=0s       med=0s      max=15.5ms   p(90)=0s      p(95)=0s
     http_req_waiting...............: avg=755.07µs min=200.8µs  med=880.7µs max=15.92ms  p(90)=1ms     p(95)=1ms
     http_reqs......................: 9001    37.440313/s
     iteration_duration.............: avg=1s       min=1s       med=1s      max=1.02s    p(90)=1s      p(95)=1s
     iterations.....................: 9001    37.440313/s
     vus............................: 1       min=1       max=50
     vus_max........................: 50      min=50      max=50
```

This is what the traffic graph for this test looks like:

![](/img/performance/load-test.jpg)

The benefits of having ramp-up, sustain and ramp-down stages are:

- It allows the system to warm up to the users
- It allows any auto scale to handle the traffic and scale to the new traffic requirements
- It allows us to compare the response times between the low traffic requests and the high traffic requests

### Performance thresholds

You should keep in mind that the thresholds you use to determine a "success" or a "failure" are application specific
and can go beyond simple ones such as "99% of all requests are successful".


## Exercise: Write a load test simulating a normal day

Normal days usually have variable loads. What we want to do is write a load test that simulates that.
The use-case below assumes that 20 concurrent users is the normal system traffic.

The stages should be:

1. Simulate ramp-up to 20 users over 5 minutes
2. Stay at 20 users for 10 minutes
3. Simulate ramp-up to 50 users over 3 minutes to simulate increasing traffic
4. Stay at 50 users for 2 minutes to sustain the increased load
5. Simulate ramp-down to 20 users over 3 minutes to simulate decreasing traffic
6. Stay at 20 users for 10 minutes
7. Simulate ramp-down to 0 users

The graph for the test should look as follows:

![](/img/performance/load-test-2.jpg)
