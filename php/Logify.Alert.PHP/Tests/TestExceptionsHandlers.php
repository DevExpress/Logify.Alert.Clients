<?php

require_once(__DIR__ . '/clientForTest.php');

class ExceptionsHandlerTest extends PHPUnit_Framework_TestCase {

    private $client;
    protected function setUp() {
        $this->client = new LogifyAlertTestClient();
        $GLOBALS['LogifyAlertClient'] = $this->client;
        $this->client->pathToConfigFile = __DIR__ . '/configForTest.php';
    }
    public function testSendExceptionMessage() {
        $message = 'test exception';
        $this->client->exception_handler(new Exception($message));
        $report = json_decode($this->client->get_saved_reports()[0], true);
        $this->assertEquals($message, $report['exception'][0]['message']);
    }
    public function testSendError() {
        $this->client->error_handler(2, 'test error', 'testfile.err', 101);
        $report = json_decode($this->client->get_saved_reports()[0], true);
        $this->assertEquals('ErrorException', $report['exception'][0]['type']);
    }
    public function testSendErrorSeverity() {
        $severity = 2;
        $this->client->error_handler($severity, 'test error', 'testfile.err', 101);
        $report = json_decode($this->client->get_saved_reports()[0], true);
        $this->assertEquals($severity, $report['exception'][0]['severity']);
    }
    public function testSendErrorMessage() {
        $message = 'test error';
        $this->client->error_handler(2, $message, 'testfile.err', 101);
        $report = json_decode($this->client->get_saved_reports()[0], true);
        $this->assertEquals($message, $report['exception'][0]['message']);
    }
    public function testSendErrorFile() {
        $file = 'testfile.err';
        $this->client->error_handler(2, 'test error', $file, 101);
        $report = json_decode($this->client->get_saved_reports()[0], true);
        $this->assertEquals($file, $report['exception'][0]['file']);
    }
    public function testSendErrorLine() {
        $line = 101;
        $this->client->error_handler(2, 'test error', 'testfile.err', $line);
        $report = json_decode($this->client->get_saved_reports()[0], true);
        $this->assertEquals($line, $report['exception'][0]['line']);
    }
}