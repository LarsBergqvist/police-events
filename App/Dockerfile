# Stage 1
FROM node:10-alpine as build-step
RUN mkdir -p /app
WORKDIR /app
COPY package.json /app
RUN npm install
COPY . /app
RUN npm run build --prod
COPY src/assets/app-config.prod.json /app/dist/police-events/assets/app-config.json
# Stage 2
FROM nginx:1.17.1-alpine
COPY certificates /etc/nginx/certificates
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build-step /app/dist/police-events /usr/share/nginx/html/

