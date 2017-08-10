<?php
use DevExpress\Logify\Collectors\ReportCollector;
use DevExpress\Logify\Core\Attachment;

require_once(__DIR__ . '/../Logify/LoadHelper.php');
spl_autoload_register(array("DevExpress\LoadHelper", "LoadModule"));

class StructureTest extends PHPUnit_Framework_TestCase {

    private $report;
    private $reportData;
    protected function setUp() {
        $this->report = new ReportCollector(new Exception('test exception'), array(
            'get' => true,
            'post' => true,
            'cookie' => true,
            'files' => true,
            'environment' => true,
            'request' => true,
            'server' => true,
                ), true, 'testuser', 'tests', 't.0');
        $_SERVER['HTTP_USER_AGENT'] = 'testuseragent';
        $this->reportData = $this->report->CollectData();
    }
    public function testReportStructure() {
        $this->assertTrue(is_array($this->reportData));
    }
    public function testReportStructureAllInclude() {
        $this->assertEquals(11, count($this->reportData));
    }
    public function testReportStructureLogifyProtocolVersion() {
        $this->assertTrue(array_key_exists('logifyProtocolVersion', $this->reportData));
    }
    public function testReportStructureLogifyReportDateTimeUtc() {
        $this->assertTrue(array_key_exists('logifyReportDateTimeUtc', $this->reportData));
    }
    public function testReportStructureLogifyAPP() {
        $this->assertTrue(array_key_exists('logifyApp', $this->reportData));
    }
    public function testReportStructureLogifyException() {
        $this->assertTrue(array_key_exists('exception', $this->reportData));
    }
    public function testReportStructureLogifyPHPLoadedExtensions() {
        $this->assertTrue(array_key_exists('PHPLoadedExtensions', $this->reportData));
    }
    public function testReportStructureLogifyGlobals() {
        $this->assertTrue(array_key_exists('globals', $this->reportData));
    }
    public function testReportStructureLogifyOS() {
        $this->assertTrue(array_key_exists('os', $this->reportData));
    }
    public function testReportStructureLogifyMemory() {
        $this->assertTrue(array_key_exists('memory', $this->reportData));
    }
    public function testReportStructureLogifyDevPlatform() {
        $this->assertTrue(array_key_exists('devPlatform', $this->reportData));
    }
    public function testReportStructureLogifyUserAgent() {
        $this->assertTrue(array_key_exists('useragent', $this->reportData));
    }
    public function testReportStructureLogifyPHPVersion() {
        $this->assertTrue(array_key_exists('phpversion', $this->reportData));
    }
    public function testReportStructureCustomData() {
        $this->report->AddCustomData('customData');
        $this->reportData = $this->report->CollectData();
        $this->assertEquals(12, count($this->reportData));
    }
    public function testReportStructureAttachment() {
        $attachment = new Attachment();
        $attachment->content = 'testAttachment';
        $attachment->mimeType = 'mime/text';
        $attachment->name = 'text';
        $this->report->AddAttachments(array($attachment));
        $this->reportData = $this->report->CollectData();
        $this->assertEquals(12, count($this->reportData));
    }
    public function testReportStructureAll() {
        $attachment = new Attachment();
        $attachment->content = 'testAttachment';
        $attachment->mimeType = 'mime/text';
        $attachment->name = 'text';

        $this->report->AddCustomData('customData');
        $this->report->AddAttachments(array($attachment));
        $this->reportData = $this->report->CollectData();
        $this->assertEquals(13, count($this->reportData));
    }
    public function testReportStructureCollectExtensions() {
        $attachment = new Attachment();
        $attachment->content = 'testAttachment';
        $attachment->mimeType = 'mime/text';
        $attachment->name = 'text';
        $this->report = new ReportCollector(new Exception('test exception'), array(
            'get' => true,
            'post' => true,
            'cookie' => true,
            'files' => true,
            'environment' => true,
            'request' => true,
            'server' => true,
                ), false, 'testuser', 'tests', 't.0');
        $_SERVER['HTTP_USER_AGENT'] = 'testuseragent';
        $this->report->AddCustomData('customData');
        $this->report->AddAttachments(array($attachment));

        $this->reportData = $this->report->CollectData();
        $this->assertEquals(12, count($this->reportData));
    }
    public function testReportCustomData() {
        $this->report->AddCustomData(array('custom1' => 'data1', 'custom2' => 'data2'));
        $this->reportData = $this->report->CollectData();
        $this->assertEquals(array('custom1' => 'data1', 'custom2' => 'data2'), $this->reportData['customData']);
    }
}
?>