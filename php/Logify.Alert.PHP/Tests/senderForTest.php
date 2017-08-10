<?php
use DevExpress\Logify\Core\ReportSender;

class ReportSenderTest extends ReportSender {
    public $savedReports = array();
    protected function send_core($json) {
        $message = json_decode($json, true)['exception'][0]['message'];
        return $message;
    }
    protected function save_report($json) {
        $this->savedReports[] = $json;
    }
}
