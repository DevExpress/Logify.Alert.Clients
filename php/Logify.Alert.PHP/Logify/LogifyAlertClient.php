<?php
namespace DevExpress\Logify;

use DevExpress\Logify\Collectors\ReportCollector;
use LogifyAlert;
use DevExpress\Logify\Core\ReportSender;

class LogifyAlertClient {

    #region static
    public static function get_instance() {
        if (!array_key_exists('LogifyAlertClient', $GLOBALS)) {
            $GLOBALS['LogifyAlertClient'] = new LogifyAlertClient();
        }
        return $GLOBALS['LogifyAlertClient'];
    }
    #endregion
    #region handlers
    private $beforeReportException = null;
    private $afterReportException = null;
    private $canReportException = null;
    #endregion
    #region Properties
    public $apiKey;
    public $appName;
    public $appVersion;
    public $attachments = null;
    public $customData = null;
    public $userId;
    public $globalVariablesPermissions = array();
    public $pathToConfigFile = '/config.php';
    public $serviceUrl;
    public $collectExtensions = null;
    public $offlineReportsCount = null;
    public $offlineReportsDirectory = '';
    public $offlineReportsEnabled = null;
    
    protected $sender = null;
    #endregion
    public function send($exception, $customData = null, $attachments = null) {
        $response = 0;
        $canReportException = $this->canReportException === null ? true : call_user_func($this->canReportException, $exception);
        if ($canReportException) {
            $this->rise_before_report_exception_callback();
            $this->configure();
            $this->create_report_sender();
            $report = $this->get_report_collector($exception, $customData, $attachments);
            $response = $this->sender->send($report->CollectData());
            $this->rise_after_report_exception_callback($response);
            return $response;
        }
        return $response;
    }
    public function send_offline_reports() {
        $this->configure();
        $this->create_report_sender();
        $this->sender->send_offline_reports();
    }
    #region Exception Handlers
    public function start_exceptions_handling() {
        set_exception_handler(array($this, 'exception_handler'));
        set_error_handler(array($this, 'error_handler'));
    }
    public function stop_exceptions_handling() {
        restore_exception_handler();
        restore_error_handler();
    }
    public function error_handler($severity, $message, $file, $line) {
        if (!(error_reporting() & $severity)) {
            return;
        }
        $this->send(new \ErrorException($message, 0, $severity, $file, $line));
    }
    public function exception_handler($exception) {
        $this->send($exception);
    }
    #endregion
    #region Configure
    protected function configure() {
        if (!file_exists($this->pathToConfigFile)) {
            return;
        }
        $included = include_once($this->pathToConfigFile);
        if (!$included) {
            return;
        }
        $configs = new LogifyAlert();
        if (property_exists($configs, 'settings')) {
            $this->configureSettings($configs->settings);
        }
        $this->configureProperties($configs);
        if (property_exists($configs, 'globalVariablesPermissions')) {
            $this->configureGlobalVariablesPermissions($configs);
        }
    }
    protected function get_report_collector($exception, $customData = null, $attachments = null) {
        $report = new ReportCollector($exception, $this->globalVariablesPermissions, $this->collectExtensions, $this->userId, $this->appName, $this->appVersion);
        $report->AddCustomData($customData !== null ? $customData : $this->customData);
        $report->AddAttachments($attachments !== null ? $attachments : $this->attachments);
        return $report;
    }
    private function configureSettings($settings) {
        if ($settings === null) {
            return;
        }
        if (empty($this->apiKey) && key_exists('apiKey', $settings)) {
            $this->apiKey = $settings['apiKey'];
        }
        if (empty($this->serviceUrl) && key_exists('serviceUrl', $settings)) {
            $this->serviceUrl = $settings['serviceUrl'];
        }
        if (empty($this->userId) && key_exists('userId', $settings)) {
            $this->userId = $settings['userId'];
        }
        if (empty($this->appName) && key_exists('appName', $settings)) {
            $this->appName = $settings['appName'];
        }
        if (empty($this->appVersion) && key_exists('appVersion', $settings)) {
            $this->appVersion = $settings['appVersion'];
        }
    }
    private function configureProperties($configs) {
        if ($this->collectExtensions === null && property_exists($configs, 'collectExtensions') && $configs->collectExtensions !== null) {
            $this->collectExtensions = $configs->collectExtensions;
        }
        if ($this->offlineReportsCount === null && property_exists($configs, 'offlineReportsCount') && $configs->offlineReportsCount !== null) {
            $this->offlineReportsCount = $configs->offlineReportsCount;
        }
        if (empty($this->offlineReportsDirectory) && property_exists($configs, 'offlineReportsDirectory')) {
            $this->offlineReportsDirectory = $configs->offlineReportsDirectory;
        }
        if ($this->offlineReportsEnabled === null && property_exists($configs, 'offlineReportsEnabled') && $configs->offlineReportsEnabled !== null) {
            $this->offlineReportsEnabled = $configs->offlineReportsEnabled;
        }
    }
    private function configureGlobalVariablesPermissions($configs) {
        if (!is_array($this->globalVariablesPermissions)) {
            $this->globalVariablesPermissions = array();
        }
        $this->collectGlobalVariablesPermissions('get', $configs);
        $this->collectGlobalVariablesPermissions('post', $configs);
        $this->collectGlobalVariablesPermissions('cookie', $configs);
        $this->collectGlobalVariablesPermissions('files', $configs);
        $this->collectGlobalVariablesPermissions('environment', $configs);
        $this->collectGlobalVariablesPermissions('request', $configs);
        $this->collectGlobalVariablesPermissions('server', $configs);
    }
    private function collectGlobalVariablesPermissions($name, $configs) {
        if (!array_key_exists($name, $configs->globalVariablesPermissions) || $configs->globalVariablesPermissions[$name] === null) {
            return;
        }
        if (!array_key_exists($name, $this->globalVariablesPermissions) || $this->globalVariablesPermissions[$name] === null) {
            $this->globalVariablesPermissions[$name] = $configs->globalVariablesPermissions[$name];
        }
    }
    #endregion
    #region Sender
    protected function create_report_sender(){
        $this->sender = new ReportSender($this->apiKey, $this->serviceUrl);
    }
    #endregion
    #region CanReportExceptionCallback
    public function set_can_report_exception_callback(callable $canReportExceptionHandler) {
        $this->canReportException = $canReportExceptionHandler;
    }
    #endregion
    #region BeforeReportExceptionCallback
    public function set_before_report_exception_callback(callable $beforeReportExceptionHandler) {
        $this->beforeReportException = $beforeReportExceptionHandler;
    }
    protected function rise_before_report_exception_callback() {
        if ($this->beforeReportException !== null) {
            call_user_func($this->beforeReportException);
        }
    }
    #endregion
    #region AfterReportExceptionCallback
    public function set_after_report_exception_callback(callable $afterReportExceptionHandler) {
        $this->afterReportException = $afterReportExceptionHandler;
    }
    protected function rise_after_report_exception_callback($response) {
        if ($this->afterReportException !== null) {
            call_user_func($this->afterReportException, $response);
        }
    }
    #endregion
}
?>