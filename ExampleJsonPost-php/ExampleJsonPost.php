<?php

$baseUrl = "https://optimizer2.routesavvy.com/RSAPI.svc/";
$POST = true;

try {
    if (count($argv) != 3) {
        echo "Usage: php script.php <input json request file> <output json result file>\n";
        echo "Press any key to exit.\n";
        readline();
        exit;
    }

    $inFilePath = $argv[1];
    $outFilePath = $argv[2];

    $requestStr = file_get_contents($inFilePath);
    $requestStr = str_replace("\t", "", $requestStr);

    $endpoint = "POSTOptimize";
    $requestUrl = $baseUrl . $endpoint;

    $httpClient = new \GuzzleHttp\Client();

    if ($POST) {
        $response = $httpClient->post($requestUrl, [
            'headers' => ['Content-Type' => 'application/json'],
            'body' => $requestStr
        ]);
    } else {
        // For GET request
        $requestUrl = $baseUrl . "GETOptimize?query=" . $requestStr;
        $response = $httpClient->get($requestUrl);
    }

    $statusCode = $response->getStatusCode();
    $responseBody = $response->getBody()->getContents();

    if ($statusCode == 200) {
        $responseObject = json_decode($responseBody);

        // Access properties of the response object
        $message = $responseObject->Message;

        if ($message !== null && $message !== "Success") {
            echo "An error occurred: $message\n";
        } else {
            try {
                $outputJson = json_decode($responseBody, true);
                file_put_contents($outFilePath, json_encode($outputJson, JSON_PRETTY_PRINT));
                echo "Success\n";
            } catch (Exception $ex) {
                echo "An error occurred: " . $ex->getMessage() . "\n";
            }
        }
    } else {
        echo "Error: $statusCode - " . $response->getReasonPhrase() . "\n";
    }
} catch (Exception $e) {
    echo "Exception: " . $e->getMessage() . "\n";
}

echo "Press any key to exit.\n";
readline();
