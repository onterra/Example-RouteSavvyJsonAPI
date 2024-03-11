import sys
import os
import requests
import json

async def main(args):
    base_url = "https://optimizer2.routesavvy.com/RSAPI.svc/"

    try:
        if len(args) != 3:
            print("Usage: <input json request file> <output json result file>")
            input("Press Enter to exit.")
            return

        in_file_path = args[1]
        out_file_path = args[2]

        with open(in_file_path, 'r') as file:
            request_str = file.read().replace("\t", "")

        endpoint = "POSTOptimize"
        request_url = f"{base_url}{endpoint}"

        headers = {"Content-Type": "application/json"}
        response = requests.post(request_url, data=request_str, headers=headers)

        if response.status_code == 200:
            response_body = response.json()
            with open(out_file_path, 'w') as file:
                json.dump(response_body, file, indent=4)
            print("Success")
        else:
            print(f"Error: {response.status_code} - {response.reason}")

    except Exception as e:
        print(f"Exception: {str(e)}")

    input("Press Enter to exit.")

if __name__ == "__main__":
    asyncio.run(main(sys.argv))
