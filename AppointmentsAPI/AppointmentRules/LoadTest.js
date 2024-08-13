import http from 'k6/http';

export const options = {
    vus: 10,
    duration: '60s',
    thresholds: {
        http_req_failed: ['rate<0.01'], // http errors should be less than 1%
        http_req_duration: ['p(95)<200'], // 95% of requests should be below 200ms
    },
};

export default function () {
    let data = {
        memberId: 1,
        taskId:1,
        startTime: new Date().toString(),
        endTime: new Date().toString(),

    };
    let response = http.post('https://localhost:7274/api/TimeEntry/Create', JSON.stringify(data), {
        headers: { 'Content-Type': 'application/json' }
    });
    check(response, { 'is status 200': (r) => r.status === 200});
    sleep(1);
}
