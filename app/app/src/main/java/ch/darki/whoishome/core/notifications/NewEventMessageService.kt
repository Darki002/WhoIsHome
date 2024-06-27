package ch.darki.whoishome.core.notifications

import android.util.Log
import com.google.firebase.messaging.FirebaseMessagingService
import com.google.firebase.messaging.RemoteMessage

class NewEventMessageService : FirebaseMessagingService() {

    override fun onNewToken(token: String) {
        super.onNewToken(token)
        Log.d("NewEventMessageService", "Refreshed token: $token")
    }

    override fun onMessageReceived(remoteMessage: RemoteMessage) {
        super.onMessageReceived(remoteMessage)
        Log.d("NewEventMessageService", "Message data: ${remoteMessage.data}")
        // Display push notification
    }

    private fun notify(remoteMessage: RemoteMessage) {
        
    }
}