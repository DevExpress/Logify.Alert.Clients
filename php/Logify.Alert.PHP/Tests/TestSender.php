<?php

require_once(__DIR__ . '/clientForTest.php');

class SenderTest extends PHPUnit_Framework_TestCase {

    private $client;
    protected function setUp() {
        $this->client = new LogifyAlertTestClient();
        $GLOBALS['LogifyAlertClient'] = $this->client;
        $this->client->pathToConfigFile = __DIR__ . '/configForTest.php';
        $this->client->offlineReportsEnabled = true;
    }
    public function testOfflineCountException() {
        for ($i = 0; $i < 5; $i++) {
            $this->client->send(new Exception('test exception'));
        }
        $reports = $this->client->get_saved_reports();
        $this->assertEquals(5, count($reports));
    }
    public function testOfflineException() {
        $this->client->send(new Exception('test exception'));
        $report = json_decode($this->client->get_saved_reports()[0], true);
        $this->assertEquals('test exception', $report['exception'][0]['message']);
    }
    public function testOfflineExceptionFalse() {
        $this->client->offlineReportsEnabled = false;
        $this->client->send(new Exception('test exception'));
        $reports = $this->client->get_saved_reports();
        $this->assertEquals(0, count($reports));
    }
    public function testOfflineExceptionCount0() {
        $this->client->offlineReportsCount = 0;
        $this->client->send(new Exception('test exception'));
        $reports = $this->client->get_saved_reports();
        $this->assertEquals(0, count($reports));
    }
}