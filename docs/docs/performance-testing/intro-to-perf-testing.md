---
description: Let's understand what is performance testing
---

# Introduction to Performance Testing

## What is Performance testing?

Performance testing is a non-functional testing technique which aims to identify the performance of our system.
"Performance" is usually quantified by speed, stability, scalability and responsiveness.

It aims to answer questions such as:

- How is my system performing under normal load?
- How is my system performing under normal load for an extended period of time?
- How is my system performing when 10000 users use it all at once?
- How is my system scaling to handle increased traffic and how does it scale down when the traffic is gone?
- How will my system behave if it's hit with 100.000 requests per second

All of these are questions that performance testing can answer.

## Different types of performance testing

There are different types of performance or load testing. The tooling is the same but what changes is the way 
we send the traffic to the target. 

Here are the main types of load testing:

![](/img/performance/test-types.jpg)

Each test type and approach aims to give us specific information about how our system runs under load.

- Smoke Tests help us verify how our app performs during expected, normal load.
- Load Tests help us verify how our system performs during expected, high load
- Stress Tests help us verify how our system performs during unexpected, sustained high load
- Spike Tests help us verify how our system performs during unexpected, transient high load
- Soak Tests help us verify how reliable our system is during load over a long period of time

## Introducing k6

For our performance testing we will be using a tool called [k6](https://k6.io/).

It is an open-source load testing tool written in Go and using the CLI to run and JavaScript to script out and makes the process of writing load tests extremely simple.

## Installation

### Windows

If you use the [Chocolatey package manager](https://chocolatey.org/) you can install the unofficial k6 package with:

```commandline
choco install k6
```

If you use the [Windows Package Manager](https://github.com/microsoft/winget-cli), install the official packages from the k6 manifests:

```commandline
winget install k6
```

Alternatively, you can download and run [the latest official installer](https://dl.k6.io/msi/k6-latest-amd64.msi).

### MacOS

Using [Homebrew](https://brew.sh/):

```commandline
brew install k6
```

### Linux

#### Debian/Ubuntu

```commandline
sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update
sudo apt-get install k6
```

#### Fedora/CentOS

Using `dnf` (or `yum` on older versions):

```commandline
sudo dnf install https://dl.k6.io/rpm/repo.rpm
sudo dnf install k6
```
