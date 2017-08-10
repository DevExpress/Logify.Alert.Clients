<?php
use DevExpress\Logify\Core\Attachment;

require_once(__DIR__ . '/clientForTest.php');

class ConfigTest extends PHPUnit_Framework_TestCase {

    private $client;
    protected function setUp() {
        $this->client = new LogifyAlertTestClient();
        $this->client->pathToConfigFile = __DIR__ . '/configForTest.php';
    }
    public function testConfigApiKey() {
        $this->client->configureCall();
        $this->assertEquals('configApiKey', $this->client->apiKey);
    }
    public function testClientApiKey() {
        $this->client->apiKey = 'clientApikey';
        $this->client->configureCall();
        $this->assertEquals('clientApikey', $this->client->apiKey);
    }
    public function testConfigUrl() {
        $this->client->configureCall();
        $this->assertEquals('configUrl', $this->client->serviceUrl);
    }
    public function testClientUrl() {
        $this->client->serviceUrl = 'clientUrl';
        $this->client->configureCall();
        $this->assertEquals('clientUrl', $this->client->serviceUrl);
    }
    public function testConfigUserId() {
        $this->client->configureCall();
        $this->assertEquals('configUserId', $this->client->userId);
    }
    public function testClientUserId() {
        $this->client->userId = 'clientUserId';
        $this->client->configureCall();
        $this->assertEquals('clientUserId', $this->client->userId);
    }
    public function testConfigPermissionsGet() {
        $this->client->configureCall();
        $this->assertTrue($this->client->globalVariablesPermissions['get']);
    }
    public function testClientPermissionsGet() {
        $this->client->globalVariablesPermissions['get'] = false;
        $this->client->configureCall();
        $this->assertFalse($this->client->globalVariablesPermissions['get']);
    }
    public function testConfigPermissionsPost() {
        $this->client->configureCall();
        $this->assertTrue($this->client->globalVariablesPermissions['post']);
    }
    public function testClientPermissionsPost() {
        $this->client->globalVariablesPermissions['post'] = false;
        $this->client->configureCall();
        $this->assertFalse($this->client->globalVariablesPermissions['post']);
    }
    public function testConfigPermissionsCookie() {
        $this->client->configureCall();
        $this->assertTrue($this->client->globalVariablesPermissions['cookie']);
    }
    public function testClientPermissionsCookie() {
        $this->client->globalVariablesPermissions['cookie'] = false;
        $this->client->configureCall();
        $this->assertFalse($this->client->globalVariablesPermissions['cookie']);
    }
    public function testConfigPermissionsFiles() {
        $this->client->configureCall();
        $this->assertTrue($this->client->globalVariablesPermissions['files']);
    }
    public function testClientPermissionsFiles() {
        $this->client->globalVariablesPermissions['files'] = false;
        $this->client->configureCall();
        $this->assertFalse($this->client->globalVariablesPermissions['files']);
    }
    public function testConfigPermissionsEnvironment() {
        $this->client->configureCall();
        $this->assertTrue($this->client->globalVariablesPermissions['environment']);
    }
    public function testClientPermissionsEnvironment() {
        $this->client->globalVariablesPermissions['environment'] = false;
        $this->client->configureCall();
        $this->assertFalse($this->client->globalVariablesPermissions['environment']);
    }
    public function testConfigPermissionsRequest() {
        $this->client->configureCall();
        $this->assertTrue($this->client->globalVariablesPermissions['request']);
    }
    public function testClientPermissionsRequest() {
        $this->client->globalVariablesPermissions['request'] = false;
        $this->client->configureCall();
        $this->assertFalse($this->client->globalVariablesPermissions['request']);
    }
    public function testConfigPermissionsServer() {
        $this->client->configureCall();
        $this->assertTrue($this->client->globalVariablesPermissions['server']);
    }
    public function testClientPermissionsServer() {
        $this->client->globalVariablesPermissions['server'] = false;
        $this->client->configureCall();
        $this->assertFalse($this->client->globalVariablesPermissions['server']);
    }
    public function testConfigAppName() {
        $this->client->configureCall();
        $this->assertEquals('testsConfig', $this->client->appName);
    }
    public function testCLientAppName() {
        $this->client->appName = 'clientTests';
        $this->client->configureCall();
        $this->assertEquals('clientTests', $this->client->appName);
    }
    public function testConfigAppVersion() {
        $this->client->configureCall();
        $this->assertEquals('t.c.0', $this->client->appVersion);
    }
    public function testCLientAppVersion() {
        $this->client->appVersion = 'client.t.0';
        $this->client->configureCall();
        $this->assertEquals('client.t.0', $this->client->appVersion);
    }
    public function testConfigCollectExtensions() {
        $this->client->configureCall();
        $this->assertTrue($this->client->collectExtensions);
    }
    public function testCLientCollectExtensions() {
        $this->client->collectExtensions = false;
        $this->client->configureCall();
        $this->assertFalse($this->client->collectExtensions);
    }
    public function testConfigOfflineReportsEnabled() {
        $this->client->configureCall();
        $this->assertTrue($this->client->offlineReportsEnabled);
    }
    public function testCLientOfflineReportsEnabled() {
        $this->client->offlineReportsEnabled = false;
        $this->client->configureCall();
        $this->assertFalse($this->client->offlineReportsEnabled);
    }
    public function testConfigOfflineReportsCount() {
        $this->client->configureCall();
        $this->assertEquals(20, $this->client->offlineReportsCount);
    }
    public function testCLientOfflineReportsCount() {
        $this->client->offlineReportsCount = 10;
        $this->client->configureCall();
        $this->assertEquals(10, $this->client->offlineReportsCount);
    }
    public function testConfigOfflineReportsDirectory() {
        $this->client->configureCall();
        $this->assertEquals('configDir', $this->client->offlineReportsDirectory);
    }
    public function testCLientOfflineReportsDirectory() {
        $this->client->offlineReportsDirectory = 'clientDir';
        $this->client->configureCall();
        $this->assertEquals('clientDir', $this->client->offlineReportsDirectory);
    }
    public function testClientCustomData() {
        $this->client->customData = array('testCustomData' => 'clientCustomData');
        $reportData = $this->client->getReport(null, null)->CollectData();
        $this->assertEquals('clientCustomData', $reportData['customData']['testCustomData']);
    }
    public function testCallBackCustomData() {
        $this->client->set_before_report_exception_callback(array($this, 'before_callback'));
        $reportData = $this->client->getReport(null, null)->CollectData();
        $this->assertEquals('callbackCustomData', $reportData['customData']['testCustomData']);
    }
    public function testSendCustomData() {
        $this->client->set_before_report_exception_callback(array($this, 'before_callback'));
        $this->client->customData = array('testCustomData' => 'clientCustomData');
        $reportData = $this->client->getReport(array('testCustomData' => 'sendCustomData'), null)->CollectData();
        $this->assertEquals('sendCustomData', $reportData['customData']['testCustomData']);
    }
    public function testClientAttachmentsName() {
        $this->client->attachments = $this->getAttachments('testClientAttachmnet', 'test/client', 'client');
        $reportData = $this->client->getReport(null, null)->CollectData();
        $this->assertEquals('testClientAttachmnet', $reportData['attachments'][0]['name']);
    }
    public function testCallBackAttachmentsName() {
        $this->client->set_before_report_exception_callback(array($this, 'before_callback'));
        $reportData = $this->client->getReport(null, null)->CollectData();
        $this->assertEquals('testCallbackAttachment', $reportData['attachments'][0]['name']);
    }
    public function testSendAttachmentsName() {
        $this->client->set_before_report_exception_callback(array($this, 'before_callback'));
        $this->client->attachments = $this->getAttachments('testClientAttachmnet', 'test/client', 'client');
        $reportData = $this->client->getReport(null, $this->getAttachments('testSendAttachmnet', 'test/send', 'send'))->CollectData();
        $this->assertEquals('testSendAttachmnet', $reportData['attachments'][0]['name']);
    }
    public function testClientAttachmentsMimeType() {
        $this->client->attachments = $this->getAttachments('testClientAttachmnet', 'test/client', 'client');
        $reportData = $this->client->getReport(null, null)->CollectData();
        $this->assertEquals('test/client', $reportData['attachments'][0]['mimeType']);
    }
    public function testCallBackAttachmentsMimeType() {
        $this->client->set_before_report_exception_callback(array($this, 'before_callback'));
        $reportData = $this->client->getReport(null, null)->CollectData();
        $this->assertEquals('test/callback', $reportData['attachments'][0]['mimeType']);
    }
    public function testSendAttachmentsMimeType() {
        $this->client->set_before_report_exception_callback(array($this, 'before_callback'));
        $this->client->attachments = $this->getAttachments('testClientAttachmnet', 'test/client', 'client');
        $reportData = $this->client->getReport(null, $this->getAttachments('testSendAttachmnet', 'test/send', 'send'))->CollectData();
        $this->assertEquals('test/send', $reportData['attachments'][0]['mimeType']);
    }
    public function testClientAttachmentsContent() {
        $this->client->attachments = $this->getAttachments('testClientAttachmnet', 'test/client', 'client');
        $reportData = $this->client->getReport(null, null)->CollectData();
        $this->assertEquals(base64_encode(gzencode('client', 9)), $reportData['attachments'][0]['content']);
    }
    public function testCallBackAttachmentsContent() {
        $this->client->set_before_report_exception_callback(array($this, 'before_callback'));
        $reportData = $this->client->getReport(null, null)->CollectData();
        $this->assertEquals(base64_encode(gzencode('callback', 9)), $reportData['attachments'][0]['content']);
    }
    public function testSendAttachmentsContent() {
        $this->client->set_before_report_exception_callback(array($this, 'before_callback'));
        $this->client->attachments = $this->getAttachments('testClientAttachmnet', 'test/client', 'client');
        $reportData = $this->client->getReport(null, $this->getAttachments('testSendAttachmnet', 'test/send', 'send'))->CollectData();
        $this->assertEquals(base64_encode(gzencode('send', 9)), $reportData['attachments'][0]['content']);
    }
    function before_callback() {
        $this->client->customData = array('testCustomData' => 'callbackCustomData');
        $this->client->attachments = $this->getAttachments('testCallbackAttachment', 'test/callback', 'callback');
    }
    function getAttachments($name, $mimeType, $content) {
        $attachment = new Attachment();
        $attachment->name = $name;
        $attachment->mimeType = $mimeType;
        $attachment->content = $content;
        return array($attachment);
    }
}
?>