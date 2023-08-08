FROM node:20-alpine AS build

WORKDIR /app
COPY . .
RUN NODE_ENV=development npm install
RUN npm run build

FROM node:20-alpine AS deploy-node

EXPOSE 3000
ENV NODE_ENV=production
ENV PROTOCOL_HEADER=x-forwarded-proto
ENV HOST_HEADER=x-forwarded-host
ENV AUTH_TRUST_HOST=true

WORKDIR /app
RUN rm -rf ./*
COPY --from=build /app/package.json .
COPY --from=build /app/build .

CMD ["node", "index.js"]
