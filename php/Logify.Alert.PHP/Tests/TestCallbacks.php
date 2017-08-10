<?php

require_once(__DIR__ . '/clientForTest.php');

class CallbacksTest extends PHPUnit_Framework_TestCase {

    private $client;
    private $callbackMessagePull;
    private $canReportExceptionReturn;
    
    protected function setUp() {
        $this->client = new LogifyAlertTestClient();
        $GLOBALS['LogifyAlertClient'] = $this->client;
        $this->client->pathToConfigFile = __DIR__ . '/configForTest.php';
    }
    public function testCanReportExceptionCallable() {
        $testMessage = 'testCanReportException';
        $this->canReportExceptionReturn = true;
        $this->client->set_can_report_exception_callback(array($this, 'can_report_exception'));
        $this->client->send(new Exception($testMessage));
        $this->assertEquals($testMessage, $this->callbackMessagePull);
    }
    public function testCanReportExceptionReturnTrue() {
        $this->canReportExceptionReturn = true;
        $this->client->set_can_report_exception_callback(array($this, 'can_report_exception'));
        $this->client->send(new Exception(''));
        $reports = $this->client->get_saved_reports();
        $this->assertEquals(1, count($reports));
    }
    public function testCanReportExceptionReturnFalse() {
        $this->canReportExceptionReturn = false;
        $this->client->set_can_report_exception_callback(array($this, 'can_report_exception'));
        $this->client->send(new Exception(''));
        $this->assertFalse($this->client->have_saved_reports());
    }
    
    public function testBeforeReportExceptionCallable() {
        $testMessage = 'testBeforeReportException';
        $this->client->set_before_report_exception_callback(array($this, 'before_report_exception'));
        $this->client->send(new Exception($testMessage));
        $this->assertEquals($testMessage, $this->callbackMessagePull);
    }
    
    public function testAfterReportExceptionCallable() {
        $testMessage = 'testAfterReportException';
        $this->client->set_after_report_exception_callback(array($this, 'after_report_exception'));
        $this->client->send(new Exception($testMessage));
        $this->assertEquals($testMessage, $this->callbackMessagePull);
    }

    function can_report_exception($exception){
        $this->callbackMessagePull = $exception->getMessage();
        return $this->canReportExceptionReturn;
    }
    function before_report_exception(){
        $this->callbackMessagePull = 'testBeforeReportException';
    }
    function after_report_exception($response) {
        $this->callbackMessagePull = $response;
    }
}