<?php 
/**
 * Plugin Name:Tableau Embed Plugin
 * Description:A simple plugin for embedding Tableau Workbooks into Wordpress with JWT authentication written in native PHP
 * Version 1.0
 * */
function base64url_encode($data) {
    $encoded = base64_encode($data);
    $encoded = str_replace(['+', '/', '='], ['-', '_', ''], $encoded);
    return $encoded;
}


function tableau_jwt_shortcode($atts) 
{
    // Viz url passed from the shortcode block
    $viz_url = $atts['viz_url'];

    // Setting the url for the API source based on the viz url domain
    $tmp = explode("/", $viz_url);
    $domain = $tmp[2];
    $api_path = '/javascripts/api/tableau.embedding.3.latest.min.js';
    $api_url = 'https://' . $domain . $api_path;

    // Define static values from Tableau Connected App
    $username = 'john.keegan86@gmail.com';
    // Connected App Client ID
    $iss = 'Connected App Client ID';
    // Connected App Secret ID;
    $kid = 'Connected App Secret ID';
    // Connected App Secret Key
    $key = 'Connected App Secret Key';
    // Expiration time in minutes this is just the JWT exp not session
    $expiration = 5; 

	// Headers for JWT
    $header = [
        'typ' => 'JWT',
        'alg' => 'HS256',
        'kid' => $kid,
        'iss' => $iss
        
    ];
    // Encode header
    $stringifiedHeader = json_encode($header);
    $encodedHeader = base64url_encode($stringifiedHeader);
 
    // Set JWT Payload attributes, 
    // May want to set more varialbes i.e. Groups and ODA 
    // Scopes may need additional designations
    // see documentation for more information,
    $payload = [
        'iss' => $iss,
        'exp' => time() + (60*$expiration), 
        'jti' => uniqid(),
        'aud' => 'tableau',
        'sub' => $username,
        'scp' => ["tableau:views:embed"]
    ];
     
    // Encode payload
    $stringifiedData = json_encode($payload);
    $encodedData = base64url_encode($stringifiedData);
     
    // Build token
    $token = $encodedHeader . '.' . $encodedData;
     
    // Sign the JWT, this is meant to only be done on the backend for security purposes
    $signature = hash_hmac('sha256', $token, $key, true);
    $encodedSignature = base64url_encode($signature);
    $jwt = $token . '.' . $encodedSignature;    


    //Starting object capture
    //all html content to embed goes into this block
    ob_start(); 
    ?> 


    <script type='module' src='<?php echo $api_url; ?>'>
    </script> 

        <tableau-viz
            id='tableauViz'
            src='<?php echo $viz_url; ?>'
            token='<?php echo $jwt; ?>'
            toolbar='hidden' hide-tabs width=100%>
        </tableau-viz>

    <?php 
    // Ending object capture and returning the content to the page
    $viz_tag = ob_get_contents();

    ob_end_clean();

    return $viz_tag;

}
//Add the shortcode function to the plugin or theme's function.php file
// shortcode example: [tableau viz_url='Dashboard Link']
add_shortcode('tableau', 'tableau_jwt_shortcode');

 ?>