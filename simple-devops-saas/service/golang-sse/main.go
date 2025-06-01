package main

import (
	"fmt"
	"net/http"
	"time"
)

func sseHandler(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "text/event-stream")
	w.Header().Set("Cache-Control", "no-cache")
	w.Header().Set("Connection", "keep-alive")

	flusher, ok := w.(http.Flusher)
	if !ok {
		http.Error(w, "Streaming unsupported!", http.StatusInternalServerError)
		return
	}

	for i := 0; ; i++ {
		fmt.Fprintf(w, "data: Hello %d at %s\n\n", i, time.Now().Format(time.RFC3339))
		flusher.Flush()
		time.Sleep(2 * time.Second)
	}
}

func main() {
	http.HandleFunc("/events", sseHandler)
	fmt.Println("SSE server started at http://localhost:8080/events")
	http.ListenAndServe(":8080", nil)
}
