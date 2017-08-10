<?php
use DevExpress\Logify\LogifyAlertClient;

require_once(__DIR__ . '/../Logify/LoadHelper.php');
spl_autoload_register(array("DevExpress\LoadHelper", "LoadModule"));

require_once(__DIR__ . '/senderForTest.php');

class LogifyAlertTestClient extends LogifyAlertClient {

    public function configureCall() {
        $this->configure();
    }
    public function getReport($customData, $attachments) {
        $this->rise_before_report_exception_callback();
        return $this->get_report_collector(new Exception(), $customData, $attachments);
    }
    public function get_saved_reports(){
        return $this->sender->savedReports;
    }
    protected function create_report_sender() {
        if ($this->sender === null) {
            $this->sender = new ReportSenderTest($this->apiKey, $this->serviceUrl);
        }
    }
    public function have_saved_reports(){
        if($this->sender !== null && $this->sender->savedReports !== null && count($this->sender->savedReports)>0){
            return true;
        } else{
            return false;
        }
    }
}
?>