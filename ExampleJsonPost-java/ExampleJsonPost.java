import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.concurrent.CompletableFuture;

import org.apache.http.HttpEntity;
import org.apache.http.client.methods.CloseableHttpResponse;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.HttpClients;
import org.json.JSONObject;

public class Main {

    public static void main(String[] args) {
        final String baseUrl = "https://optimizer2.routesavvy.com/RSAPI.svc/";

        try {
            if (args.length != 2) {
                System.out.println("Usage: <input json request file> <output json result file>");
                System.out.println("Press any key to exit.");
                System.in.read();
                return;
            }

            String inFilePath = args[0];
            String outFilePath = args[1];

            String requestStr = new String(Files.readAllBytes(Paths.get(inFilePath))).replaceAll("\\t", "");

            String endpoint = "POSTOptimize";
            String requestUrl = baseUrl + endpoint;

            try (CloseableHttpClient httpClient = HttpClients.createDefault()) {
                HttpPost httpPost = new HttpPost(requestUrl);
                httpPost.setHeader("Content-Type", "application/json");

                StringEntity stringEntity = new StringEntity(requestStr);
                httpPost.setEntity(stringEntity);

                CompletableFuture<CloseableHttpResponse> responseFuture = CompletableFuture.supplyAsync(() -> {
                    try {
                        return httpClient.execute(httpPost);
                    } catch (IOException e) {
                        throw new RuntimeException(e);
                    }
                });

                CloseableHttpResponse response = responseFuture.get();
                try {
                    int statusCode = response.getStatusLine().getStatusCode();
                    if (statusCode == 200) {
                        HttpEntity entity = response.getEntity();
                        String responseBody = entity != null ? new String(entity.getContent().readAllBytes()) : null;

                        JSONObject jsonObject = new JSONObject(responseBody);
                        String message = jsonObject.getString("Message");

                        if (message != null && !message.equals("Success")) {
                            System.out.println("An error occurred: " + message);
                        } else {
                            try {
                                JSONObject outputJson = new JSONObject(responseBody);
                                Files.write(Paths.get(outFilePath), outputJson.toString(4).getBytes());
                                System.out.println("Success");
                            } catch (Exception ex) {
                                System.out.println("An error occurred: " + ex.getMessage());
                            }
                        }
                    } else {
                        System.out.println("Error: " + statusCode + " - " + response.getStatusLine().getReasonPhrase());
                    }
                } finally {
                    response.close();
                }
            }
        } catch (Exception e) {
            System.out.println("Exception: " + e.getMessage());
        }

        System.out.println("Press any key to exit.");
        try {
            System.in.read();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
