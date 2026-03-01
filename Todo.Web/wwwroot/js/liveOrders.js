let eventSource = null;

export function startLiveOrders(url, dotNetRef) {
    if (eventSource) return;

    eventSource = new EventSource(url);

    eventSource.addEventListener("order", (event) => {
        const json = JSON.parse(event.data);
        dotNetRef.invokeMethodAsync("OnOrderEvent", json);
    });

    eventSource.onerror = (error) => {
        console.error("EventSource failed:", error);
        stopLiveOrders();
        dotNetRef.invokeMethodAsync("OnSseError", "EventSource failed");
    };
}

export function stopLiveOrders() {
    if (!eventSource) return;
    eventSource.close();
    eventSource = null;
}