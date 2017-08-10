<?php
namespace DevExpress\Logify\Core;

use DevExpress\Logify\LogifyAlertClient;

class ReportSender {

    public $apiKey;
    public $serviceUrl = 'http://logify.devexpress.com/api/report/newreport';
    function __construct($apiKey, $serviceUrl = null) {
        $this->apiKey = $apiKey;
        if ($serviceUrl != null) {
            $this->serviceUrl = $serviceUrl;
        }
    }
    function send($data) {
        $json = json_encode($data);
        $result = true;
        for ($i = 1; $i <= 3; $i++) {
            $result = $this->send_core($json);
            if ($result === true) {
                break;
            }
        }
        $client = LogifyAlertClient::get_instance();
        if ($result !== true && $client->offlineReportsEnabled === true && $client->offlineReportsCount !== null && $client->offlineReportsCount > 0) {
            $this->save_report($json);
        }
        return $result;
    }
    function send_offline_reports() {
        $client = LogifyAlertClient::get_instance();
        $files = $this->get_saved_repots_filenames($client->offlineReportsDirectory);
        if (is_array($files)) {
            foreach ($files as $filename) {
                $json = file_get_contents($filename);
                if ($this->send_core($json) === true) {
                    unlink($filename);
                }
            }
        }
    }
    protected function send_core($json) {
        $header = $this->generate_header(strlen($json));
        $request = curl_init();
        $errorMessage = '';
        curl_setopt_array($request, [
            CURLOPT_URL => $this->serviceUrl,
            CURLOPT_RETURNTRANSFER => true,
            CURLOPT_HTTPHEADER => $header,
            CURLOPT_POST => true,
            CURLOPT_POSTFIELDS => $json,
            CURLOPT_FOLLOWLOCATION => true
        ]);
        try {
            $response = curl_exec($request);
            if ($response === false) {
                $errorMessage = curl_error($request);
            }
            curl_close($request);
        } catch (Exception $e) {
            return $e;
        }
        if (empty($errorMessage)) {
            return true;
        }
        return $errorMessage;
    }
    private function generate_header($content_length) {
        $header = array(
            'Authorization: amx ' . $this->apiKey,
            'Content-Type: application/json',
            'Content-Length: ' . $content_length,
        );
        return $header;
    }
    protected function save_report($json) {
        $client = LogifyAlertClient::get_instance();
        $this->free_unnecessary_file($client->offlineReportsDirectory, $client->offlineReportsCount);
        $filename = tempnam($client->offlineReportsDirectory, 'LR_');
        if ($filename !== false) {
            file_put_contents($filename, $json);
        }
    }
    private function free_unnecessary_file($directory, $maxReportsCount) {
        $files = $this->get_saved_repots_filenames($directory);
        if (is_array($files)) {
            $deleteCount = count($files) - $maxReportsCount + 1;
            if ($deleteCount > 0) {
                $deletefiles = array();
                for ($i = 0; $i < count($files); $i++) {
                    $deletefiles[$files[$i]] = filemtime($files[$i]);
                }
                asort($deletefiles);
                $deletefiles = array_slice(array_keys($deletefiles), 0, $deleteCount);
                for ($i = 0; $i < $deleteCount; $i++) {
                    unlink($deletefiles[$i]);
                }
            }
        }
    }
    private function get_saved_repots_filenames($directory) {
        return glob($directory . DIRECTORY_SEPARATOR . 'LR_*.*');
    }
}
?>