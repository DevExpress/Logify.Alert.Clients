<?php
class LogifyAlert {

    public $settings = array(
        'serviceUrl' => 'configUrl',
        'apiKey' => 'configApiKey',
        'userId' => 'configUserId',
        'appName' => 'testsConfig',
        'appVersion' => 't.c.0',
    );
    public $collectExtensions = true;
    public $offlineReportsEnabled = true;
    public $offlineReportsCount = 20;
    public $offlineReportsDirectory = 'configDir';
    public $globalVariablesPermissions = array(
        'get' => true,
        'post' => true,
        'cookie' => true,
        'files' => true,
        'environment' => true,
        'request' => true,
        'server' => true,
    );
}