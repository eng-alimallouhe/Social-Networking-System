# المرحلة الأولى: البناء
FROM node:22 AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build --configuration=production

# المرحلة الثانية: Nginx
FROM nginx:alpine

# نسخ ملف الإعدادات الخاص بنا لتجاوز 404
COPY nginx.conf /etc/nginx/conf.d/default.conf

# نسخ ملفات البناء
COPY --from=build /app/dist/SNS.Web/browser /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]