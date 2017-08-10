<?php
use DevExpress\Logify\Collectors\ReportCollector;

require_once(__DIR__ . '/../Logify/LoadHelper.php');
spl_autoload_register(array("DevExpress\LoadHelper", "LoadModule"));

class PermissionsTest extends PHPUnit_Framework_TestCase {

    private $report;
    protected function setUp() {
        $_GET['test'] = 'test';
        $_POST['test'] = 'test';
        $_COOKIE['test'] = 'test';
        $_FILES['test'] = 'test';
        $_ENV['test'] = 'test';
        $_REQUEST['test'] = 'test';
        $_SERVER['test'] = 'test';
    }
    public function testReportGlobalsNoGetStructure() {
        $this->report = new ReportCollector(new Exception('test exception'), array(
            'get' => false,
            'post' => true,
            'cookie' => true,
            'files' => true,
            'environment' => true,
            'request' => true,
            'server' => true,
                ), true, 'testuser', 'tests', 't.0');
        $this->assertFalse(array_key_exists('get', $this->report->CollectData()['globals']));
    }
    public function testReportGlobalsNoPostStructure() {
        $this->report = new ReportCollector(new Exception('test exception'), array(
            'get' => true,
            'post' => false,
            'cookie' => true,
            'files' => true,
            'environment' => true,
            'request' => true,
            'server' => true,
                ), true, 'testuser', 'tests', 't.0');
        $this->assertFalse(array_key_exists('post', $this->report->CollectData()['globals']));
    }
    public function testReportGlobalsNoCookieStructure() {
        $this->report = new ReportCollector(new Exception('test exception'), array(
            'get' => true,
            'post' => true,
            'cookie' => false,
            'files' => true,
            'environment' => true,
            'request' => true,
            'server' => true,
                ), true, 'testuser', 'tests', 't.0');
        $this->assertFalse(array_key_exists('cookie', $this->report->CollectData()['globals']));
    }
    public function testReportGlobalsNoFilesStructure() {
        $this->report = new ReportCollector(new Exception('test exception'), array(
            'get' => true,
            'post' => true,
            'cookie' => true,
            'files' => false,
            'environment' => true,
            'request' => true,
            'server' => true,
                ), true, 'testuser', 'tests', 't.0');
        $this->assertFalse(array_key_exists('files', $this->report->CollectData()['globals']));
    }
    public function testReportGlobalsNoEnvironmentStructure() {
        $this->report = new ReportCollector(new Exception('test exception'), array(
            'get' => true,
            'post' => true,
            'cookie' => true,
            'files' => true,
            'environment' => false,
            'request' => true,
            'server' => true,
                ), true, 'testuser', 'tests', 't.0');
        $this->assertFalse(array_key_exists('environment', $this->report->CollectData()['globals']));
    }
    public function testReportGlobalsNoRequestStructure() {
        $this->report = new ReportCollector(new Exception('test exception'), array(
            'get' => true,
            'post' => true,
            'cookie' => true,
            'files' => true,
            'environment' => true,
            'request' => false,
            'server' => true,
                ), true, 'testuser', 'tests', 't.0');
        $this->assertFalse(array_key_exists('request', $this->report->CollectData()['globals']));
    }
    public function testReportGlobalsNoServerStructure() {
        $this->report = new ReportCollector(new Exception('test exception'), array(
            'get' => true,
            'post' => true,
            'cookie' => true,
            'files' => true,
            'environment' => true,
            'request' => true,
            'server' => false,
                ), true, 'testuser', 'tests', 't.0');
        $this->assertFalse(array_key_exists('server', $this->report->CollectData()['globals']));
    }
}
?>