import requests
from requests.auth import HTTPBasicAuth

RABBITMQ_USER = "guest"
RABBITMQ_PASSWORD = "guest"
RABBITMQ_HOST = "http://localhost:15672"

auth = HTTPBasicAuth(RABBITMQ_USER, RABBITMQ_PASSWORD)

def delete_all_queues():
    response = requests.get(f"{RABBITMQ_HOST}/api/queues", auth=auth)
    queues = response.json()

    for queue in queues:
        vhost = queue["vhost"]
        queue_name = queue["name"]
        delete_url = f"{RABBITMQ_HOST}/api/queues/{vhost}/{queue_name}"
        requests.delete(delete_url, auth=auth)
        print(f"Deleted queue: {queue_name}")

def delete_all_exchanges():
    response = requests.get(f"{RABBITMQ_HOST}/api/exchanges", auth=auth)
    exchanges = response.json()

    for exchange in exchanges:
        vhost = exchange["vhost"]
        exchange_name = exchange["name"]
        if exchange_name not in ["", "amq.default"]:  # Avoid default exchanges
            delete_url = f"{RABBITMQ_HOST}/api/exchanges/{vhost}/{exchange_name}"
            requests.delete(delete_url, auth=auth)
            print(f"Deleted exchange: {exchange_name}")

# Execute deletions
delete_all_queues()
delete_all_exchanges()
