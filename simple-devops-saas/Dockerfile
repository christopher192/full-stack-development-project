FROM golang:1.20-alpine AS builder

WORKDIR /app

COPY golang-sse/main.go .

RUN go build -o sse-server main.go

FROM alpine:latest

WORKDIR /app

COPY --from=builder /app/sse-server .

EXPOSE 8080

CMD ["./sse-server"]