import axios from 'axios';

export const myAxios = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
  timeout: 1000,
});

/*
  日期解析器，形如 "2021-09-01T12:34:56.789Z" 的字符串会被解析为 Date 对象
 */
const parseDates = (data: any) => {
  if (data && typeof data === 'object') {
    for (const key in data) {
      if (Object.prototype.hasOwnProperty.call(data, key)) {
        const value = data[key];
        if (
          typeof value === 'string' &&
          /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z$/.test(value)
        ) {
          data[key] = new Date(value);
        } else if (typeof value === 'object') {
          parseDates(value);
        }
      }
    }
  }
  return data;
};

// 添加响应拦截器
myAxios.interceptors.response.use((response) => {
  response.data = parseDates(response.data);
  return response;
});
