---
description: Let's see how we can implement smoke tests with k6
---

# Smoke testing

## Smoke testing the Customers API

Let's start by writing the simplest smoke test for the Customers API that we used during the Integration testing section.

Here is what the base for our k6 test looks like:

```javascript
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    vus: 1, 
    duration: '1m'
};

const BASE_URL = 'https://localhost:5001';

export default () => {

    sleep(1);
};
```

In the `options` object we configure how we want our test to run. In the `export default ()` function we implement the code that interact with our service.

VUs stands for virtual users. In Smoke Tests we only want to work with 1-2 VUs.

The `sleep` function accepts seconds as a parameter so in this specific test, the action will be performed by one virtual user who will be calling the API once every second. 

Now let's go ahead and implement a GET request against the `https://localhost:5001/customers` endpoint that returns all customers.

```javascript
export default () => {
    http.get(`${BASE_URL}/customers/`);
    sleep(1);
};
```

And that's it! Technically, this is enough to run a very basic smoke test.

Simply run the following command and wait for 1 minute for the test to complete.

```commandline
k6 run ./smoke-test.js
```

The results should look something like this:

```commandline
          /\      |‾‾| /‾‾/   /‾‾/
     /\  /  \     |  |/  /   /  /
    /  \/    \    |     (   /   ‾‾\
   /          \   |  |\  \ |  (‾)  |
  / __________ \  |__| \__\ \_____/ .io

  execution: local
     script: ./smoke-test.js
     output: -

  scenarios: (100.00%) 1 scenario, 1 max VUs, 1m30s max duration (incl. graceful stop):
           * default: 1 looping VUs for 1m0s (gracefulStop: 30s)

running (1m00.4s), 0/1 VUs, 60 complete and 0 interrupted iterations
default ✓ [======================================] 1 VUs  1m0s

     data_received..................: 20 kB  323 B/s
     data_sent......................: 3.2 kB 53 B/s
     http_req_blocked...............: avg=299.99µs min=0s      med=0s       max=17.99ms p(90)=0s       p(95)=0s
     http_req_connecting............: avg=0s       min=0s      med=0s       max=0s      p(90)=0s       p(95)=0s
   ✓ http_req_duration..............: avg=1.22ms   min=846.4µs med=1.21ms   max=3.49ms  p(90)=1.4ms    p(95)=1.58ms
       { expected_response:true }...: avg=1.22ms   min=846.4µs med=1.21ms   max=3.49ms  p(90)=1.4ms    p(95)=1.58ms
     http_req_failed................: 0.00%  ✓ 0        ✗ 60
     http_req_receiving.............: avg=209.63µs min=0s      med=272.3µs  max=511µs   p(90)=416.49µs p(95)=470.86µs
     http_req_sending...............: avg=16.65µs  min=0s      med=0s       max=999.3µs p(90)=0s       p(95)=0s
     http_req_tls_handshaking.......: avg=250.01µs min=0s      med=0s       max=15ms    p(90)=0s       p(95)=0s
     http_req_waiting...............: avg=1ms      min=831.9µs med=949.75µs max=2.49ms  p(90)=1ms      p(95)=1.42ms
     http_reqs......................: 60     0.993349/s
     iteration_duration.............: avg=1s       min=1s      med=1s       max=1.02s   p(90)=1s       p(95)=1s
     iterations.....................: 60     0.993349/s
     vus............................: 1      min=1      max=1
     vus_max........................: 1      min=1      max=1
```

This is what the the smoke testing load can be visualized as:

![](/img/performance/smoke-test.jpg)

Now even though this is a valid test technically, we aren't validating anything related to performance.

This is where thresholds and checks come in.

### Thresholds

In the `options` object we can specify a `thresholds` object. 
In this object we can define limits that the tests should stay inside of to be considered a successful run.

For example if we wanted 99% of the requests to respond below 15 milliseconds, we should have a `thresholds` object with the following value:

```js
thresholds: {
    http_req_duration: ['p(99)<15']
}
```

There are multiple types of thresholds that k6 supports, which can be found [here](https://k6.io/docs/using-k6/thresholds/)

### Checks

Checks is a way to validate that the data returned by the system we are testing is what we expect it to be.
You can think of them as assertions for our performance tests. For example, we have 1 customer in the database. 
I want to write a check that ensures that this one user is returned in every response.

```js
const customers = http.get(`${BASE_URL}/customers/`).json();
check(customers, { 'retrieved customers': (obj) => obj.customers.length > 0 });
```

### Running the test properly

This is our complete smoke test code now:

```js title="smoke-test.js"
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

And these are the results its producing:

```commandline
          /\      |‾‾| /‾‾/   /‾‾/
     /\  /  \     |  |/  /   /  /
    /  \/    \    |     (   /   ‾‾\
   /          \   |  |\  \ |  (‾)  |
  / __________ \  |__| \__\ \_____/ .io

  execution: local
     script: ./smoke-test.js
     output: -

  scenarios: (100.00%) 1 scenario, 1 max VUs, 1m30s max duration (incl. graceful stop):
           * default: 1 looping VUs for 1m0s (gracefulStop: 30s)
           
running (1m00.5s), 0/1 VUs, 60 complete and 0 interrupted iterations
default ✓ [======================================] 1 VUs  1m0s

     ✓ retrieved customers

     checks.........................: 100.00% ✓ 60       ✗ 0
     data_received..................: 20 kB   322 B/s
     data_sent......................: 3.2 kB  53 B/s
     http_req_blocked...............: avg=300µs    min=0s      med=0s      max=18ms    p(90)=0s       p(95)=0s
     http_req_connecting............: avg=8.33µs   min=0s      med=0s      max=500µs   p(90)=0s       p(95)=0s
   ✓ http_req_duration..............: avg=1.22ms   min=853.8µs med=1.25ms  max=1.53ms  p(90)=1.46ms   p(95)=1.5ms
       { expected_response:true }...: avg=1.22ms   min=853.8µs med=1.25ms  max=1.53ms  p(90)=1.46ms   p(95)=1.5ms
     http_req_failed................: 0.00%   ✓ 0        ✗ 60
     http_req_receiving.............: avg=214.4µs  min=0s      med=278.2µs max=505µs   p(90)=433.07µs p(95)=467.66µs
     http_req_sending...............: avg=8.33µs   min=0s      med=0s      max=500.1µs p(90)=0s       p(95)=0s
     http_req_tls_handshaking.......: avg=241.66µs min=0s      med=0s      max=14.5ms  p(90)=0s       p(95)=0s
     http_req_waiting...............: avg=999.67µs min=853.1µs med=999.7µs max=1.5ms   p(90)=1ms      p(95)=1.38ms
     http_reqs......................: 60      0.992178/s
     iteration_duration.............: avg=1s       min=1s      med=1s      max=1.03s   p(90)=1s       p(95)=1s
     iterations.....................: 60      0.992178/s
     vus............................: 1       min=1      max=1
     vus_max........................: 1       min=1      max=1
```

## Dealing with authentication

Most systems require the users to be authenticated before they can make actions.
Since the performance tests run from the perspective of users, they also need to be authenticated.

This is something we need to do as part of the main test function. 

So for example if we needed to call the `/auth/token/login` endpoint, pass down some credentials, check that the response was successful,
and then use the returned access token to our API request, it could look like this:

```js
export default () => {
    const loginRes = http.post(`${BASE_URL}/auth/token/login/`, {
        username: "nick",
        password: "SecretPass69",
    });

    check(loginRes, {
        'logged in successfully': (resp) => resp.json('access') !== '',
    });

    const authHeaders = {
        headers: {
            Authorization: `Bearer ${loginRes.json('access')}`,
        },
    };
    
    
    const customers = http.get(`${BASE_URL}/customers/`, authHeaders).json();
    check(customers, { 'retrieved customers': (obj) => obj.customers.length > 0 });
    sleep(1);
};
```

Since authentication has so many different flavours, it is up to you to script it out for each individual scenario.

And that's it! Assuming that you see no errors, your system is robust enough to go to the next step, Load Testing.
